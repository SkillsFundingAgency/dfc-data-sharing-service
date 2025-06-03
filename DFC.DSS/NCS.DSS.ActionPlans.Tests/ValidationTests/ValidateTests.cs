using Moq;
using DSS.ActionPlan.Validation;
using NUnit.Framework;
using DSS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSS.Models.Enums;

namespace DSS.ActionPlan.Tests.ValidationTests
{
    [TestFixture]
    public class ValidateTests
    {

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanCreatedIsNotSupplied()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(1, result);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenSessionIdIsNotSupplied()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow,
                CustomerCharterShownToCustomer = false,
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(1, result);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanCreatedIsNotGreaterThanDateAndTimeOfSession()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, DateTime.MaxValue);

            // Assert
            ExecuteAssert(1, result);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanCreatedIsInTheFuture()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                CustomerCharterShownToCustomer = false,
                DateActionPlanCreated = DateTime.MaxValue,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(1, result);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenCustomerCharterShownToCustomerIsNotSupplied()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(1, result);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateAndTimeCharterShownIsInTheFuture()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow,
                SessionId = Guid.Empty,
                CustomerCharterShownToCustomer = false,
                DateAndTimeCharterShown = DateTime.MaxValue
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(1, result);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanSentToCustomerIsInTheFuture()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty,
                DateActionPlanSentToCustomer = DateTime.MaxValue
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(1, result);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanSentToCustomerIsNotGreaterThanDateActionPlanCreated()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow.AddDays(2),
                CustomerCharterShownToCustomer = true,
                SessionId = Guid.Empty,
                DateActionPlanSentToCustomer = DateTime.UtcNow
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(2, result);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanAcknowledgedIsInTheFuture()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow,
                SessionId = Guid.Empty,
                CustomerCharterShownToCustomer = false,
                DateActionPlanAcknowledged = DateTime.MaxValue
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(1, result);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanAcknowledgedIsNotGreaterThanDateActionPlanCreated()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.MaxValue,
                SessionId = Guid.Empty,
                DateActionPlanAcknowledged = DateTime.UtcNow
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(3, result);

        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenLastModifiedDateIsInTheFuture()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty,
                LastModifiedDate = DateTime.MaxValue
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(1, result);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenActionPlanDeliveryMethodIsNotValid()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty,
                ActionPlanDeliveryMethod = (ActionPlanDeliveryMethod)100
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, It.IsAny<DateTime>());

            // Assert
            ExecuteAssert(1, result);

        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanCreatedIsLessThanDateTimeSessionCreated()
        {

            var actionPlan = new DSS.Models.ActionPlan
            {
                CustomerCharterShownToCustomer = false,
                DateActionPlanCreated = DateTime.UtcNow,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, DateTime.UtcNow.AddDays(2));

            // Assert
            ExecuteAssert(1, result);

        }
        private void ExecuteAssert(int resultCount, object result)
        {
            // Assert
            Assert.That(typeof(List<ValidationResult>) == result.GetType());
            Assert.That(result, Is.Not.Null);
            Assert.That(resultCount == ((List<ValidationResult>)result).Count);
        }

    }
}