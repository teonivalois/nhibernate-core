﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NHibernate.DomainModel;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	using System.Threading.Tasks;
	[TestFixture]
	public class DetachedCriteriaFixtureAsync : TestCase
	{
		protected override string[] Mappings
		{
			get { return new string[] {"Componentizable.hbm.xml"}; }
		}

		[Test]
		public async Task CanUseDetachedCriteriaToQueryAsync()
		{
			using (ISession s = OpenSession())
			{
				Componentizable master = new Componentizable();
				master.NickName = "master";
				await (s.SaveAsync(master));
				await (s.FlushAsync());
			}

			DetachedCriteria detachedCriteria = DetachedCriteria.For(typeof(Componentizable));
			detachedCriteria.Add(Expression.Eq("NickName", "master"));

			using (ISession s = OpenSession())
			{
				Componentizable componentizable = (Componentizable) await (detachedCriteria.GetExecutableCriteria(s).UniqueResultAsync());
				Assert.AreEqual("master", componentizable.NickName);
				await (s.DeleteAsync(componentizable));
				await (s.FlushAsync());
			}
		}
	}
}