using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using hotel_data;
using hotel_data.dto;

namespace hotel_business
{
    public class PersonBuisness
    {
        enMode mode = enMode.add;
        public Guid ID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        
        public PersonDto personData {
            get{return new PersonDto(ID, name, email,phone, address); }
        }

        public PersonBuisness(Guid id, string name,string email, string phone, string address, enMode enMode)
        {
            this.ID = id;
            this.name = name;
            this.phone = phone;
            this.address = address;
            this.mode = enMode;
            this.email = email; 
        }

        public PersonBuisness(PersonDto personData,  enMode enMode)
        {
            this.name = personData.name;
            this.phone = personData.phone;
            this.address = personData.address;
            this.email= personData.email;
            this.mode = enMode;
        }

        private bool createPerson()
        {
            return PersonData.createPerson(personData);
        }

        private bool updatePerson()
        {
            return PersonData.updatePerson(personData);
        }


        public bool save()
        {
            switch (mode)
            {
                case enMode.add:
                    {
                        if (createPerson())
                        {
                            return true;
                        }
                        return false;
                    }
                case enMode.update:
                    {
                        if (updatePerson())
                        {
                            return true;
                        }
                        return false;
                    }
                default: return false;
            }
        }

        public static PersonBuisness? getPersonByID(Guid ID)
        {
            var personData = PersonData.getPerson(ID);
            if (personData != null)
            {
                return new PersonBuisness(personData, enMode.update);
            }
            return null;
        }


        public static bool isPersonExistByID(Guid ID)
        {
            return PersonData.isExist(ID);
        }
        public static bool isPersonExistByPhone(string phone="")
        {
            return PersonData.isExist(phone);
        }
        public static bool isPersonExistByEmailAndPhone(string email,string phone)
        {
            return PersonData.isExist(email,phone);
        }
    }

}
