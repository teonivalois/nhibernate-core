﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH3564
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class MyDummyCache : CacheBase
	{

		public override Task<object> GetAsync(object key, CancellationToken cancellationToken)
		{
			try
			{
				return Task.FromResult<object>(hashtable[KeyAsString(key)]);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task PutAsync(object key, object value, CancellationToken cancellationToken)
		{
			try
			{
				hashtable[KeyAsString(key)] = value;
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task RemoveAsync(object key, CancellationToken cancellationToken)
		{
			try
			{
				hashtable.Remove(KeyAsString(key));
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task ClearAsync(CancellationToken cancellationToken)
		{
			try
			{
				hashtable.Clear();
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task<object> LockAsync(object key, CancellationToken cancellationToken)
		{
			// local cache, so we use synchronization
			return Task.FromResult<object>(null);
		}

		public override Task UnlockAsync(object key, object lockValue, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
			// local cache, so we use synchronization
		}
	}

	[TestFixture]
	public class FixtureByCodeAsync : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Property(x => x.DateOfBirth, pm =>
				{
					pm.Type(NHibernateUtil.DateTime);
				});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.CacheProvider, typeof (MyDummyCacheProvider).AssemblyQualifiedName);
			configuration.SetProperty(Environment.UseQueryCache, "true");
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Person {Name = "Bob", DateOfBirth = new DateTime(2015, 4, 22)});
				session.Save(new Person {Name = "Sally", DateOfBirth = new DateTime(2014, 4, 22)});

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public async Task ShouldUseDifferentCacheAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var bob = await (session.Query<Person>()
					.WithOptions(o => o.SetCacheable(true))
					.Where(e => e.DateOfBirth == new DateTime(2015, 4, 22))
					.ToListAsync());
				var sally = await (session.Query<Person>()
					.WithOptions(o => o.SetCacheable(true))
					.Where(e => e.DateOfBirth == new DateTime(2014, 4, 22))
					.ToListAsync());

				Assert.That(bob, Has.Count.EqualTo(1));
				Assert.That(bob[0].Name, Is.EqualTo("Bob"));

				Assert.That(sally, Has.Count.EqualTo(1));
				Assert.That(sally[0].Name, Is.EqualTo("Sally"));
			}
		}
	}
}
