using BusPark.Domain;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BusPark.DataAccess
{
    public class Repository<T> : IDisposable where T : Entity
    {
        private readonly DbProviderFactory providerFactory;
        private DbConnection connection;
        private Type type = typeof(T);

        public Repository(string connectionString, string providerInvariantName)
        {
            providerFactory = DbProviderFactories.GetFactory(providerInvariantName);
            connection = providerFactory.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
        }

        /// <summary>
        /// Добавление в БД, принимает любую сущность, если она наследуется от Entity
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"INSERT into [{type.Name}] (");

            foreach (var propertyInfo in type.GetProperties())
            {
                if (IsNullOrEmpty(propertyInfo, element))
                {
                    stringBuilder.Append($"[{propertyInfo.Name}], ");
                }
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 1);
            stringBuilder.Append(") Values (");

            foreach (var propertyInfo in type.GetProperties())
            {
                if (IsNullOrEmpty(propertyInfo, element))
                {
                    stringBuilder.Append($"@{propertyInfo.Name}, ");
                }
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 1);
            stringBuilder.Append(");");

            var sqlQuery = stringBuilder.ToString();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var rowAffected = connection.Execute(sqlQuery, element, transaction);
                    //и так далее тоже самое с другими командами
                    transaction.Commit();
                }
                catch (DbException exception)
                {
                    Console.WriteLine(exception.Message);
                    transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// Принимает Guid элемента и удаляет его из бд
        /// </summary>
        /// <param name="elementId"></param>
        public void Delete(Guid elementId)
        {
            var sqlQuery = $"update [{type.Name}] set DeletedDate = '{DateTime.Now.ToShortDateString()}' where Id = '{elementId}';";
            using (DbTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    var rowAffected = connection.Execute(sqlQuery, new { Id = elementId }, transaction);
                    //и так далее тоже самое с другими командами
                    transaction.Commit();
                }
                catch (DbException exception)
                {
                    Console.WriteLine(exception.Message);
                    transaction.Rollback();
                }
            }
        }

        public ICollection<T> GetAll()
        {
            var sqlQuery = $"Select * From {type.Name.ToString()};";
            var table = connection.Query<T>(sqlQuery).ToList();
            return table;
        }

        public void Update(T element)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Update [{type.Name}] Set ");

            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.GetValue(element) is null) continue;
                stringBuilder.Append($"@{propertyInfo.Name.ToString()} = @{propertyInfo.Name.ToString()}, ");
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 1);
            stringBuilder.Append($" Where Id = {element.Id};");
            var sqlQuery = stringBuilder.ToString();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var rawAffected = connection.Execute(sqlQuery, element, transaction);
                    //и так далее тоже самое с другими командами
                    transaction.Commit();
                }
                catch (DbException exception)
                {
                    Console.WriteLine(exception.Message);
                    transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// Проверка свойства на Nullable и хранится ли в нём значение null
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsNullOrEmpty(PropertyInfo propertyInfo, T item)
        {
            return !(propertyInfo.PropertyType.ToString().Contains("Nullable") && propertyInfo.GetValue(item) is null);
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
