using MVP.Model;
using UnityEngine;

namespace PD4.MVPBase.Presenter
{
	public interface IPresenter<T> where T : ModelBase
	{

		T Model { get; set; }

		void OnModelPropertyChanged(string propertyName);
		void OnModelUpdated(T previousModel);

	}
}
