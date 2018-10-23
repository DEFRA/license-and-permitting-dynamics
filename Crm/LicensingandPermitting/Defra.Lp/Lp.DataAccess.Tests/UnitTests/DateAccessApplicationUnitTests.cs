using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Model.Entities;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Lp.Model.EarlyBound;
using Account = Lp.Model.EarlyBound.Account;

namespace Lp.DataAccess.Tests.UnitTests
{
    /// <summary>
    /// Summary description for DateAccessApplicationUnitTests
    /// </summary>
    [TestClass]
    public class DateAccessApplicationUnitTests
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void TrialFakeXrmEasyTest()
        {
            //Ok, this is going to be our test method body

            //But before doing anything...

            //  FakeXrmEasy is based on the state-based testing paradigm, 
            //  which is made of, roughly, 3 easy steps:

            //1) We define the initial state of our test.

            //2) Then, we execute the piece of logic which we want to test, 
            //   which will produce a new state, the final state.

            //3) Finally, we verify that the final state is the expected state (assertions).

            //Let's implement those now

            // 1) Define the initial state
            // -----------------------------------------------------------------------

            //  Our initial state is going to be stored in what we call a faked context:

            var context = new XrmFakedContext();

            //You can think of a context like an Organisation database which stores entities In Memory.

            //We can also use TypedEntities but we need to tell the context where to look for them, 
            //this could be done, easily, like this:

            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Account));

            //We have to define our initial state now, 
            //by calling the Initialize method, which expects a list of entities.

            var account = new Account() { Id = Guid.NewGuid(), Name = "My First Faked Account yeah!" };

            context.Initialize(new List<Entity>() {
                account
            });

            //With the above example, we initialized our context with a single account record

            // 2) Execute our logic
            // -----------------------------------------------------------------------
            //
            // We need to get a faked organization service first, by calling this method:

            var service = context.GetOrganizationService();

            // That line is the most powerful functionality of FakeXrmEasy
            // That method has returned a reference to an OrganizationService 
            // which you could pass to your plugins, codeactivities, etc, 
            // and, from now on, every create, update, delete, even queries, etc
            // will be reflected in our In Memory context

            // In a nutshell, everything is already mocked for you... cool, isn't it?

            // Now... 

            // To illustrate this...

            // Let's say we have a super simple piece of logic which updates an account's name

            // Let's do it!

            var accountToUpdate = new Account() { Id = account.Id };
            accountToUpdate.Name = "A new faked name!";

            service.Update(accountToUpdate);

            // Done!

            //We have successfully executed the code we want to test..

            // Now...

            // The final step is...

            // 3) Verify final state is the expected state
            // -----------------------------------------------------------------------
            //

            //We are going to use Xunit assertions.

            var updatedAccountName = context.CreateQuery<Account>()
                                    .Where(e => e.Id == account.Id)
                                    .Select(a => a.Name)
                                    .FirstOrDefault();


            //And finally, validate the account has the expected name
            Assert.AreEqual("A new faked name!", updatedAccountName);
        }


    }
}
