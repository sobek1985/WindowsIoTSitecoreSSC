﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PersonSSC.Contracts;
using PersonSSC.Model;
using PersonSSC.Repositories;
using Sitecore.Analytics.DataAccess;
using Sitecore.Analytics.Tracking;

namespace PersonSSC.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        [Test]
        public void Create_Contact_Called_When_Existing_Contact_Not_Found()
        {
            var xdbContactRepository = SetupXdbContactMock(LockAttemptStatus.NotFound);

            var personRepository = new PersonRepository(xdbContactRepository.Object, null);

            personRepository.Add(new Person() { Surname = "Mr New", FirstName = "Bob" });

            xdbContactRepository.Verify(t => t.CreateContact(It.IsAny<Person>()));
        }

        [Test]
        public void Create_Contact_Not_Called_When_Existing_Contact_Found()
        {
            var xdbContactRepository = SetupXdbContactMock(LockAttemptStatus.Success);

            var personRepository = new PersonRepository(xdbContactRepository.Object, null);

            personRepository.Add(new Person() { Surname = "Mr Existing", FirstName = "Bob" });

            xdbContactRepository.Verify(t => t.CreateContact(It.IsAny<Person>()), Times.Never);
        }

        private Mock<IXdbContactRepository> SetupXdbContactMock(LockAttemptStatus requiredStatus)
        {
            var xdbContactRepository = new Mock<IXdbContactRepository>();
            var lockAttemptStatus = requiredStatus;
            xdbContactRepository.Setup(x => x.FindContact(It.IsAny<string>(), out lockAttemptStatus)).Returns((Contact)null);
            xdbContactRepository.Setup(x => x.CreateContact(It.IsAny<Person>())).Returns((Contact)null);

            return xdbContactRepository;
        }
    }
}
