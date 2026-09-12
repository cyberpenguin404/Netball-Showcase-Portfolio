using MVP.Model;
using System;
using System.ComponentModel;
using UnityEngine;

namespace MVP.Presenter
{
	public abstract class PresenterMonobehaviour<T> : MonoBehaviour where T : ModelBase
	{
		private T _model;
		public T Model
		{
			get => _model;
			set
			{
				if (value == null) throw new ArgumentNullException();
				if (_model != null && _model.Equals(value)) return;

				T prevModel = _model;
				if (_model != null)
					_model.PropertyChanged -= Model_PropertyChanged;

				_model = value;

				if (_model != null)
					_model.PropertyChanged += Model_PropertyChanged;

				OnModelUpdated(prevModel);
			}
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			OnModelPropertyChanged(args.PropertyName);
		}

		// VIRTUAL METHODS 

		/// <summary>
		/// Gets called when the Model gets set.
		/// </summary>
		/// <param name="previousModel">The model that was set before (or null)</param>
		protected virtual void OnModelUpdated(T previousModel)
		{

		}

		/// <summary>
		/// Gets called when a property of the model was changed.
		/// Works through the INotifyPropertyChanged implementation of the Model.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args">contains the PropertyName</param>
		protected virtual void OnModelPropertyChanged(string propertyName)
		{

		}

		protected virtual void Awake()
		{

		}
		protected virtual void Start()
		{

		}
		protected virtual void Update()
		{

		}


	}
}
