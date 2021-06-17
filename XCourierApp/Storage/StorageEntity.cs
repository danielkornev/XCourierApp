using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace XCourierApp.Storage
{
	public abstract class StorageEntity : INotifyPropertyChanged
	{
		#region Fields
		private string _debugMessage = string.Empty;
		private string _displayName = string.Empty;
		private bool isSoftDeleted;
		private Guid _storageId;
		#endregion

		#region Properties

		public DateTime DateCreated { get; set; }
		public DateTime DateUpdated { get; set; }
		public DateTime DateDeleted { get; set; }

		public Guid StorageId
		{
			get
			{
				if (_storageId != null)
					return _storageId;
				return Guid.NewGuid();
			}
			set
			{
				this._storageId = value;
				this.OnPropertyChanged();
			}
		}

		public bool IsSoftDeleted
		{
			get
			{
				return isSoftDeleted;
			}
			set
			{
				isSoftDeleted = value;
				this.OnPropertyChanged();
			}
		}

		public string DisplayName
		{
			get
			{
				return this._displayName;

			}
			set
			{
				this._displayName = value;
				this.OnPropertyChanged();
			}
		}

		#endregion

		public StorageEntity()
		{

		}

		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region INotifyPropertyChanged Implementation
		public virtual void Changed(string propertyName)
		{
			if (PropertyChanged != null)
			{
				var action =
					new Action
					(
						delegate
						{
							//Platform.InputInvalidate(propertyName);
							PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
						}
					);
				//Platform.InvokeInUI(action);
			}
		}

		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			// Raise the PropertyChanged event, passing the name of the property whose value has changed.
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	} // class
} // namespace
