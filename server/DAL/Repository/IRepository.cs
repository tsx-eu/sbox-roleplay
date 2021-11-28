using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace charleroi.server.DAL.Repository
{
	public interface IRepository<T> where T : class
	{
		Task<T> Get(object id);

		Task<IList<T>> GetAll();

		Task<Boolean> Insert(T entity);

		Task<Boolean> Delete(T entity);

		Task<Boolean> Update(T entity);
	}
}
