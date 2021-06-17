using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using XCourierApp.Collections.ObjectModel;

namespace XCourierApp.Storage.Journals
{
	public class JournalEntity : StorageEntity
	{
		#region Fields
		JournalPagesObservableCollection<JournalPageEntity> _pages;
		#endregion

		[BsonIgnore]
		public StorageContext StorageContext { get; internal set; }

		[BsonIgnore]
		public bool IsLoaded { get; internal set; }

		[BsonIgnore]
		public string PagesCount
		{
			get
			{
				return this.Pages.Count + " pages";
			}
		}




		#region Properties


		public Guid Id { get; set; }

		/// <summary>
		/// X coordinate on "All Journals" infinite canvas
		/// </summary>
		public double X { get; set; }

		/// <summary>
		/// Y coordinate on "All Journals" infinite canvas
		/// </summary>
		public double Y { get; set; }

		/// <summary>
		/// Zoom Level. Defines the size of the journal on "All Journals" infinite canvas.
		/// </summary>
		public double ZoomLevel { get; set; }

		/// <summary>
		/// HEX representation of the Front Cover Brush
		/// </summary>
		public string FrontCoverSolidBrushHex { get; set; }

		/// <summary>
		/// HEX representation of the Front Cover Solid Color
		/// </summary>
		public string FrontCoverSolidColorHex { get; set; }


		#endregion

		#region Temporary Properties
		/// <summary>
		/// Will be auto-calculated based on the Zoom level. Keeping it here for now.
		/// </summary>
		public double Width { get; set; }

		#endregion

		#region Constructor
		public JournalEntity()
		{
			if (string.IsNullOrEmpty(this.DisplayName))
				this.DisplayName = "My Journal";
		}
		#endregion

		#region Collections
		/// <summary>
		/// List of all contained Journal Pages. Should be lazy loadable.
		/// </summary>
		[BsonIgnore]
		public JournalPagesObservableCollection<JournalPageEntity> Pages
		{
			get
			{
				if (this._pages == null)
				{
					this._pages = new JournalPagesObservableCollection<JournalPageEntity>(this);

					this._pages.CollectionChanged += _pages_CollectionChanged;
				}
				return this._pages;
			}
		}

		private void _pages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			this.Changed(this.PagesCount.ToString());
		}

		public Guid LastOpenPage { get; set; }
		#endregion

		#region Methods

		public void AddTwoMorePages()
		{
			var p1 = new JournalPageEntity
			{
				DateCreated = DateTime.UtcNow,
				DateUpdated = DateTime.UtcNow,
				IsCoverPage = false,
				IsSoftDeleted = false
			};

			this.Pages.AddPage(p1);

			var p2 = new JournalPageEntity
			{
				DateCreated = DateTime.UtcNow,
				DateUpdated = DateTime.UtcNow,
				IsCoverPage = false,
				IsSoftDeleted = false
			};

			this.Pages.AddPage(p2);
		}


		#endregion
	} // class
} // namespace