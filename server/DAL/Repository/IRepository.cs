using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace charleroi.server.DAL.Repository
{
	public interface IRepository<T> where T : class
	{
		T Get(UInt64 id);

		IList<T> GetAll();

		Boolean Insert(T entity);

		Boolean Delete(T entity);

		Boolean Update(T entity);

		void Save();
	}
}
