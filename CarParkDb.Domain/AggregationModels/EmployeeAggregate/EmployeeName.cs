using System.Collections.Generic;
using CarParkDb.Domain.Models;

namespace CarParkDb.Domain.AggregationModels.EmployeeAggregate
{
    public class EmployeeName : ValueObject
    {
        public string FirstName { get;}
        public string LastName { get;}
        public string MiddleName { get; }

        public EmployeeName(string firstName, string lastName, string middleName)
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
        }

        public string MiddleNameAndInitials
        {
            get
            {
                if (MiddleName.Trim().Length > 0 && FirstName.Trim().Length > 0 && LastName.Trim().Length > 0)
                    return $"{MiddleName.Trim()} {FirstName.Trim()[0]}.{LastName.Trim()[0]}.";
                else
                    return "";
            }
        }


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
            yield return MiddleName;
        }
    }
}