using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms.Inking;

namespace XCourierApp.Storage.Journals
{
	public partial class JournalPageEntity : StorageEntity
	{
		#region Properties
		public bool IsCoverPage { get; set; }

		public string InkStrokes { get; set; }

		/// <summary>
		/// Thumbnail is important; later we'll use it to minimize loading time
		/// </summary>
		public string PageThumbnailUri { get; set; }

		public int Id { get; set; }
		#endregion

		[BsonIgnore]
		public JournalEntity Journal
		{
			get; internal set;
		}

		#region Computer Properties & Collections
		[BsonIgnore]
		public bool IsOdd
		{
			get
			{
				if (Id % 2 == 0)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}
		#endregion

		[BsonIgnore]
		public StorageContext StorageContext { get; internal set; }

		[BsonIgnore]
		public XInkStroke[] InkLayer
		{
			get
			{
				var json = this.InkStrokes;

				if (string.IsNullOrWhiteSpace(json))
				{
					return new List<XInkStroke>().ToArray();
				}

				var options = StorageContext.CreateSerializationOptions();

				XInkStroke[] inkStrokes = System.Text.Json.JsonSerializer.Deserialize<XInkStroke[]>(json, options);

				return inkStrokes;
			}
			set
			{

				var options = StorageContext.CreateSerializationOptions();

				var json = System.Text.Json.JsonSerializer.Serialize(value, options);

				// saving as JSON
				this.InkStrokes = json;
			}
		}

		public JournalPageEntity()
		{
			if (Guid.Empty == this.StorageId)
				this.StorageId = Guid.NewGuid();

			// loading ink 

			// loading objects
			// TODO
		}
	} // class
} // namespace
