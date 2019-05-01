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
    private Scope RootScope = null;
    private Scope CurrentScope;

    List<Func<T, object>> delegates = new List<Func<T, object>>();
    public void AddDelegate(Func<T, object> Delegate) => delegates.Add(Delegate);

    public void AddScope(string name)
    {
      if (RootScope == null)
      {
        RootScope = new Scope { Name = name };
        CurrentScope = RootScope;
      }
      else
      {
        Scope newScope = new Scope { Name = name, Parent = CurrentScope };
        CurrentScope.Childrens.Add(newScope);
        CurrentScope = newScope;
      }
    }

    public void EndScope() => CurrentScope = CurrentScope.Parent;
    public void AddValue(string name, Func<T, object> value) => CurrentScope.values.Add(name, value);
    public void Serialize(TextWriter writer, T instance) => RootScope.PrintYourSelf(writer, instance, 0);

    class Scope
    {
      public Scope Parent = null;
      public string Name;
      public List<Scope> Childrens = new List<Scope>();
      public Dictionary<string, Func<T, object>> values = new Dictionary<string, Func<T, object>>();
      public void PrintYourSelf(TextWriter writer, T instance, int spaces)
      {
        printSpaces(writer, spaces);
        writer.WriteLine("<" + Name + ">");
        foreach (var item in values)
        {
          printSpaces(writer, spaces + 2);
          writer.WriteLine(Utils.DelegateTextWrapper(item.Key, item.Value.Invoke(instance).ToString()));
        }
        foreach (var item in Childrens)
        {
          item.PrintYourSelf(writer, instance, spaces + 2);
        }
        printSpaces(writer, spaces);
        writer.WriteLine("</" + Name + ">");
      }
      private static void printSpaces(TextWriter writer, int spaces)
      {
        for (int i = 0; i < spaces; i++) writer.Write(" ");
      }
    }
    static class Utils
    {
      public static string DelegateTextWrapper(string name, string text) => "<" + name + ">" + text + "</" + name + ">";
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
      rootDesc.AddScope("Person");
      rootDesc.AddValue("FirstName", (Person p) => p.FirstName);
      rootDesc.AddValue("LastName", (Person p) => p.LastName);
      rootDesc.AddScope("HomeAddress");
      rootDesc.AddValue("Street", (Person p) => p.HomeAddress.Street);
      rootDesc.AddValue("City", (Person p) => p.HomeAddress.City);
      rootDesc.EndScope();
      rootDesc.AddScope("WorkAddress");
      rootDesc.AddValue("Street", (Person p) => p.WorkAddress.Street);
      rootDesc.AddValue("City", (Person p) => p.WorkAddress.City);
      rootDesc.EndScope();
      rootDesc.AddScope("CitizenOf");
      rootDesc.AddValue("Name", (Person p) => p.CitizenOf.Name);
      rootDesc.AddValue("AreaCode", (Person p) => p.CitizenOf.AreaCode);
      rootDesc.EndScope();
      rootDesc.AddScope("MobilePhone");
      rootDesc.AddValue("Number", (Person p) => p.MobilePhone.Number);
      rootDesc.AddScope("Country");
      rootDesc.AddValue("Name", (Person p) => p.MobilePhone.Country.Name);
      rootDesc.AddValue("AreaCode", (Person p) => p.MobilePhone.Country.AreaCode);
      rootDesc.EndScope();
      rootDesc.EndScope();
      rootDesc.EndScope();
      return rootDesc;
    }
  }
}
