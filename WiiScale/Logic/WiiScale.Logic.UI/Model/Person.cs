using System;
using WiiScale.Logic.UI.BaseClasses;

namespace WiiScale.Logic.UI.Model
{
    public class Person : BaseModel, ICloneable
    {

        public int Id { get; set; }
        public string Firtsname { get; set; }
        public string Lastname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ImageUrl { get; set; } = "../Assets/Person.png";
        public int? Age => DateOfBirth.HasValue ? (int)DateTime.Now.Subtract(DateOfBirth.Value).TotalDays / 364 : default(int?);
        public int? Height { get; set; }

        public object Clone()
        {
            return new Person()
            {
                Id = Id,
                Firtsname = Firtsname,
                Lastname = Lastname,
                Height = Height,
                DateOfBirth = DateOfBirth,
                ImageUrl = ImageUrl
            };
        }
    }
}