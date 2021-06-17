using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xamarin.Forms;
using XCourierApp.Collections.ObjectModel;
using XCourierApp.Droid.Storage.Serialization.Converters;
using XCourierApp.Storage.Journals;
using XCourierApp.Storage.Serialization.Converters;

namespace XCourierApp.Storage
{
	public class StorageContext
	{
		#region Fields
		private string journalsDb;
		private string courierShellDataPath;
		private LiteDatabase _storage;
		#endregion

		#region Properties
		/// <summary>
		/// Poor Man Singleton
		/// </summary>
		public static StorageContext JournalsInstance
		{
			get; private set;
		}

		/// <summary>
		/// All Journals
		/// </summary>
		public JournalsObservableCollection<JournalEntity> Journals
		{
			get; internal set;
		}
		#endregion

		public StorageContext()
		{
			journalsDb = DependencyService.Get<IStorageFileAccess>().GetStorageFileLocation();
			courierShellDataPath = System.IO.Directory.GetParent(journalsDb).FullName;

			this.Journals = new JournalsObservableCollection<JournalEntity>(this);

			// loading journals
			this.Journals.Load();

			// auto-adding first journal (seeds)
			FillJournalsWithFirstJournal();

			// setting our poor singleton
			JournalsInstance = this;
		}

		private LiteDatabase GetInternalStorageContext()
		{
			// journals database
			return new LiteDatabase(journalsDb);
		}

		private LiteDatabase GetInternalStorageContext(JournalEntity journalEntity)
		{
			if (this.Journals.Where(j => j.Id == journalEntity.Id).Count() == 0)
			{
				throw new Exception("Journal with a given Id doesn't exist");
			}

			var journalDirectoryPath = System.IO.Path.Combine(courierShellDataPath, journalEntity.Id.ToString());
			var journalDirectory = System.IO.Directory.CreateDirectory(journalDirectoryPath);

			// journal db
			var journalDb = System.IO.Path.Combine(journalDirectoryPath, "journal.db");

			return new LiteDatabase(journalDb);
		}

		#region Public Methods
		public JournalEntity CreateJournal(string displayName, string hexColor)
		{
			return this.CreateJournalInternal(Guid.NewGuid(), displayName, hexColor);
		}

		public void SaveJournal(JournalEntity journalEntity)
		{
			Exception ex;
			TrySaveJournal(journalEntity, out ex);
		}

		public void SavePage(JournalPageEntity page)
		{
			Exception ex;
			TrySavePage(page.Journal, page, out ex);
		}
		#endregion

		#region Private Methods
		private void FillJournalsWithFirstJournal()
		{
			if (this.Journals.Count(j => j.IsSoftDeleted == false) > 0) return;

			// otherwise it means we don't have any journals that are alive in the system
			CreateJournalInternal(Constants.KnownEntities.FirstJournal, "First Journal", Constants.Colors.BeautifulGreen);
		}

		private JournalEntity CreateJournalInternal(Guid id, string displayName, string hexColor)
		{
			#region Creating Journal
			var journal = new JournalEntity
			{
				Id = id,
				StorageContext = this,
				IsSoftDeleted = false,
				DateCreated = DateTime.UtcNow,
				DateUpdated = DateTime.UtcNow,
				FrontCoverSolidBrushHex = hexColor,
				FrontCoverSolidColorHex = hexColor,
				DisplayName = displayName,
				Width = 205,
				X = 350,
				Y = 490,
				ZoomLevel = 1
			};

			this.Journals.AddJournal(journal);
			#endregion

			// loading pages
			var pages = journal.Pages;

			#region Creating Front Cover & First Two Pages
			JournalPageEntity cover = new JournalPageEntity
			{
				Id = 1,
				DateCreated = DateTime.UtcNow,
				DateUpdated = DateTime.UtcNow,
				IsCoverPage = true
			};

			journal.Pages.AddPage(cover);

			JournalPageEntity page1 = new JournalPageEntity
			{
				Id = 2,
				DateCreated = DateTime.UtcNow,
				DateUpdated = DateTime.UtcNow,
				IsCoverPage = false
			};

			journal.Pages.AddPage(page1);

			JournalPageEntity page2 = new JournalPageEntity
			{
				Id = 3,
				DateCreated = DateTime.UtcNow,
				DateUpdated = DateTime.UtcNow,
				IsCoverPage = false
			};

			journal.Pages.AddPage(page2);
			#endregion

			return journal;
		}

		#endregion

		#region Journals
		internal List<JournalEntity> LoadJournals()
		{
			List<JournalEntity> journals = new List<JournalEntity>();

			// connecting to, or creating a brand new given journal's DB
			using (var db = GetInternalStorageContext())
			{
				// obtaining pages
				journals = db.GetCollection<JournalEntity>("Journals").FindAll().ToList();
			}

			// returning our pages
			return journals;
		}

		internal bool TryAddJournal(JournalEntity journalEntity, out Exception exception)
		{
			bool result = false;
			exception = null;

			try
			{
				// connecting to, or creating a brand new given journal's DB
				using (var db = GetInternalStorageContext())
				{
					// inserting a page
					db.GetCollection<JournalEntity>("Journals").Insert(journalEntity);

					// adding reference
					journalEntity.StorageContext = this;

					result = true;
				}
			}
			catch (Exception ex)
			{
				// setting our exception
				exception = ex;
				result = false;
			}

			return result;
		}

		internal bool TryMarkJournalAsDeleted(JournalEntity journalEntity, out Exception exception)
		{
			bool result = false;
			exception = null;

			try
			{
				using (var db = GetInternalStorageContext())
				{
					var id = new BsonValue(journalEntity.Id);

					// looking for page
					var pageInCollection = db.GetCollection<JournalPageEntity>("JournalPages").FindById(id);

					if (pageInCollection == null)
					{
						exception = new KeyNotFoundException("Journal doesn't exist with a given id in All Journals: " + journalEntity.DisplayName);
						result = false;
					}

					// marking as soft-deleted
					journalEntity.IsSoftDeleted = true;

					// updating
					db.GetCollection<JournalEntity>("JournalPages").Update(journalEntity);

					// we are good to go
					result = true;
				}
			}
			catch (Exception ex)
			{
				// setting our exception
				exception = ex;
				result = false;
			}

			return result;
		}

		internal bool TrySaveJournal(JournalEntity journalEntity, out Exception exception)
		{
			bool result = false;
			exception = null;

			if (!this.Journals.Exists(journalEntity))
				return TryAddJournal(journalEntity, out exception);

			try
			{
				using (var db = GetInternalStorageContext())
				{
					var id = new BsonValue(journalEntity.Id);

					// looking for page
					var pageInCollection = db.GetCollection<JournalEntity>("Journals").FindById(id);

					if (pageInCollection == null)
					{
						exception = new KeyNotFoundException("Page exists in memory, but doesn't exist in the storage in Journal: " + journalEntity.DisplayName);
						result = false;
					}

					// saving a page
					result = db.GetCollection<JournalEntity>("Journals").Update(journalEntity);

					// we are good to go
				}
			}
			catch (Exception ex)
			{
				// setting our exception
				exception = ex;
				result = false;
			}

			return result;
		}
		#endregion

		#region Pages
		internal List<JournalPageEntity> LoadPages(JournalEntity journalEntity)
		{
			List<JournalPageEntity> pages = new List<JournalPageEntity>();

			// connecting to, or creating a brand new given journal's DB
			using (var db = GetInternalStorageContext(journalEntity))
			{
				// obtaining pages
				pages = db.GetCollection<JournalPageEntity>("JournalPages").FindAll().ToList();
			}

			// returning our pages
			return pages;
		}

		internal bool TryAddPage(JournalEntity journalEntity, JournalPageEntity page, out Exception exception)
		{
			bool result = false;
			exception = null;

			try
			{
				// connecting to, or creating a brand new given journal's DB
				using (var db = GetInternalStorageContext(journalEntity))
				{
					// inserting a page
					db.GetCollection<JournalPageEntity>("JournalPages").Insert(page);

					// adding references
					page.Journal = journalEntity;

					page.StorageContext = this;

					result = true;
				}
			}
			catch (Exception ex)
			{
				// setting our exception
				exception = ex;
				result = false;
			}

			return result;
		}

		internal bool TryRemovePage(JournalEntity journalEntity, JournalPageEntity page, out Exception exception)
		{
			bool result = false;
			exception = null;

			try
			{
				// connecting to, or creating a brand new given journal's DB
				using (var db = GetInternalStorageContext(journalEntity))
				{
					var id = new BsonValue(page.Id);

					// looking for page
					var pageInCollection = db.GetCollection<JournalPageEntity>("JournalPages").FindById(id);

					if (pageInCollection == null)
					{
						exception = new KeyNotFoundException("Page doesn't exist with a given id in Journal: " + journalEntity.DisplayName);
						result = false;
					}

					// removing a page with a given Id
					db.GetCollection<JournalPageEntity>("JournalPages").Delete(id);

					// we are good to go
					result = true;
				}
			}
			catch (Exception ex)
			{
				// setting our exception
				exception = ex;
				result = false;
			}

			return result;
		}

		internal bool TrySavePage(JournalEntity journalEntity, JournalPageEntity page, out Exception exception)
		{
			bool result = false;
			exception = null;

			if (!journalEntity.Pages.Exists(page))
				return TryAddPage(journalEntity, page, out exception);

			try
			{
				// connecting to, or creating a brand new given journal's DB
				using (var db = GetInternalStorageContext(journalEntity))
				{
					var id = new BsonValue(page.Id);

					// looking for page
					var pageInCollection = db.GetCollection<JournalPageEntity>("JournalPages").FindById(id);

					if (pageInCollection == null)
					{
						exception = new KeyNotFoundException("Page exists in memory, but doesn't exist in the storage in Journal: " + journalEntity.DisplayName);
						result = false;
					}

					// saving a page
					result = db.GetCollection<JournalPageEntity>("JournalPages").Update(page);

					// we are good to go
				}
			}
			catch (Exception ex)
			{
				// setting our exception
				exception = ex;
				result = false;
			}

			return result;
		}
		#endregion

		public static JsonSerializerOptions CreateSerializationOptions()
		{
			var options = new JsonSerializerOptions();
			options.Converters.Add(new ColorConverter());
			options.Converters.Add(new SKRectConverter());
			//options.Converters.Add(new PointConverter());
			//options.Converters.Add(new DateTimeOffsetConverter()); // the default converter is sufficient

			options.WriteIndented = true;

			return options;
		}
	} // class
} // namespace
