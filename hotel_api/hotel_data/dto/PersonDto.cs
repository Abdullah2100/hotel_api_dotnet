using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hotel_data.dto
{
    public class PersonDto
    {

        public Guid?  personID { get; set; } 
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public DateTime?  createdAt { get; set; }

        public PersonDto(
            Guid? personID,
            string email, 
            string name, 
            string phone, 
            string address
            )
        {
            this.personID = personID;
            this.name = name;
            this.phone = phone;
            this.address = address;
            this.email = email;
        }

    }
}