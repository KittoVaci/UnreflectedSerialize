using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnreflectedSerializer
{

  public class RootDescriptor<T>
  {
    List<Func<T, object>> delegates = new List<Func<T, object>>();
    public void AddDelegate(Func<T, object> Delegate) => delegates.Add(Delegate);
    public void Serialize(TextWriter writer, T instance)
    {
      foreach (var Delegate in delegates)
      {
        writer.WriteLine(Delegate.Invoke(instance));
      }
    }
  }

  class Address
  {
    public string Street { get; set; }
    public string City { get; set; }
  }

  class Country
  {
    public string Name { get; set; }
    public int AreaCode { get; set; }
  }

  class PhoneNumber
  {
    public Country Country { get; set; }
    public int Number { get; set; }
  }

  class Person
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address HomeAddress { get; set; }
    public Address WorkAddress { get; set; }
    public Country CitizenOf { get; set; }
    public PhoneNumber MobilePhone { get; set; }
  }

  class Program
  {
    static void Main(string[] args)
    {
      RootDescriptor<Person> rootDesc = GetPersonDescriptor();

      var czechRepublic = new Country { Name = "Czech Republic", AreaCode = 420 };
      var person = new Person
      {
        FirstName = "Pavel",
        LastName = "Jezek",
        HomeAddress = new Address { Street = "Patkova", City = "Prague" },
        WorkAddress = new Address { Street = "Malostranske namesti", City = "Prague" },
        CitizenOf = czechRepublic,
        MobilePhone = new PhoneNumber { Country = czechRepublic, Number = 123456789 }
      };

      rootDesc.Serialize(Console.Out, person);
    }

    static RootDescriptor<Person> GetPersonDescriptor()
    {
      var rootDesc = new RootDescriptor<Person>();
      rootDesc.AddDelegate((Person person) => "<Person>");
      rootDesc.AddDelegate((Person firstName) => Utils.DelegateTextWrapper("FirstName", firstName.FirstName));
      rootDesc.AddDelegate((Person lastName) => Utils.DelegateTextWrapper("LastName", lastName.LastName));
      rootDesc.AddDelegate((Person homeAddress) => "<HomeAddress>");
      rootDesc.AddDelegate((Person street) => Utils.DelegateTextWrapper("Street", street.HomeAddress.Street));
      rootDesc.AddDelegate((Person city) => Utils.DelegateTextWrapper("City", city.HomeAddress.City));
      rootDesc.AddDelegate((Person homeAddress) => "</HomeAddress>");
      rootDesc.AddDelegate((Person homeAddress) => "<WorkAddress>");
      rootDesc.AddDelegate((Person street) => Utils.DelegateTextWrapper("Street", street.WorkAddress.Street));
      rootDesc.AddDelegate((Person city) => Utils.DelegateTextWrapper("City", city.WorkAddress.City));
      rootDesc.AddDelegate((Person homeAddress) => "</WorkAddress>");
      rootDesc.AddDelegate((Person homeAddress) => "<CitizenOf>");
      rootDesc.AddDelegate((Person name) => Utils.DelegateTextWrapper("Name", name.CitizenOf.Name));
      rootDesc.AddDelegate((Person areaCode) => Utils.DelegateTextWrapper("AreaCode", areaCode.CitizenOf.AreaCode.ToString()));
      rootDesc.AddDelegate((Person homeAddress) => "</CitizenOf>");
      rootDesc.AddDelegate((Person homeAddress) => "<MobilePhone>");
      rootDesc.AddDelegate((Person homeAddress) => "<Country>");
      rootDesc.AddDelegate((Person name) => Utils.DelegateTextWrapper("Name", name.MobilePhone.Country.Name));
      rootDesc.AddDelegate((Person areaCode) => Utils.DelegateTextWrapper("AreaCode", areaCode.MobilePhone.Country.AreaCode.ToString()));
      rootDesc.AddDelegate((Person homeAddress) => "</Country>");
      rootDesc.AddDelegate((Person number) => Utils.DelegateTextWrapper("Number", number.MobilePhone.Number.ToString()));
      rootDesc.AddDelegate((Person homeAddress) => "</MobilePhone>");
      rootDesc.AddDelegate((Person homeAddress) => "</Person>");
      return rootDesc;
    }

    static class Utils
    {
      public static string DelegateTextWrapper(string name, string text) => "<" + name + ">" + text + "</" + name + ">";
    }
  }
}
