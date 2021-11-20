using System;
using System.Collections.Generic;

namespace charleroi.server.DAL.Repository
{
	public interface IRepository<T> where T : class
	{
		T Get(object id);

		IList<T> GetAll();

		Boolean Insert(T entity);

		Boolean Delete(T entity);

		Boolean Update(T entity);

		void Save();
	}
}
