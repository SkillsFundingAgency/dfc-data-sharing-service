using Moq;
using NUnit.Framework;
using System;
using DSS.Models;

namespace DSS.ActionPlan.Tests.ModelTests
{
    [TestFixture]
    public class ActionPlanModelTests
    {

        [Test]
        public void ActionPlanTests_PopulatesDefaultValues_WhenSetDefaultValuesIsCalled()
        {
            var actionPlan = new DSS.Models.ActionPlan();
            actionPlan.SetDefaultValues();

            // Assert
            Assert.That(actionPlan.LastModifiedDate, Is.Not.Null);

            Assert.That(false == actionPlan.CustomerCharterShownToCustomer);
        }

        [Test]
        public void ActionPlanTests_CheckLastModifiedDateDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var actionPlan = new DSS.Models.ActionPlan { LastModifiedDate = DateTime.MaxValue };

            actionPlan.SetDefaultValues();

            // Assert
            Assert.That(DateTime.MaxValue == actionPlan.LastModifiedDate);
        }

        [Test]
        public void ActionPlanTests_CheckCustomerCharterShownToCustomerDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var actionPlan = new DSS.Models.ActionPlan { CustomerCharterShownToCustomer = true };

            actionPlan.SetDefaultValues();

            // Assert
            Assert.That(true == actionPlan.CustomerCharterShownToCustomer);
        }

        [Test]
        public void ActionPlanTests_CheckActionPlanIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new DSS.Models.ActionPlan();

            actionPlan.SetIds(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.That(actionPlan.ActionPlanId, Is.Not.Empty);// != Guid.Empty);
        }

        [Test]
        public void ActionPlanTests_CheckCustomerIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new DSS.Models.ActionPlan();

            var customerId = Guid.NewGuid();
            actionPlan.SetIds(customerId, It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.That(customerId == actionPlan.CustomerId);
        }

        [Test]
        public void ActionPlanTests_CheckInteractionIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new DSS.Models.ActionPlan();

            var interactionId = Guid.NewGuid();
            actionPlan.SetIds(It.IsAny<Guid>(), interactionId, It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.That(interactionId == actionPlan.InteractionId);
        }

        [Test]
        public void ActionPlanTests_CheckTouchpointIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new DSS.Models.ActionPlan();

            actionPlan.SetIds(It.IsAny<Guid>(), It.IsAny<Guid>(), "0000000000", It.IsAny<string>());

            // Assert
            Assert.That("0000000000" == actionPlan.LastModifiedTouchpointId);
        }

        [Test]
        public void ActionPlanTests_CheckSubcontractorIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new DSS.Models.ActionPlan();

            actionPlan.SetIds(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), "0000000000");

            // Assert
            Assert.That("0000000000" == actionPlan.SubcontractorId);
        }

    }
}