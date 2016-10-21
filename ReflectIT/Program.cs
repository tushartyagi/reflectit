using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectIT
{
    class Program
    {
        static void Main(string[] args)
        {
            // Solves: How do I create a type that needs generic arguments and 
            // I want to specify the arguments separately?
            var employeeList = CreateCollection(typeof(List<>), typeof(Employee));
            Console.WriteLine(employeeList.GetType().Name);
            Console.WriteLine(employeeList.GetType().FullName);

            // The first four lines show how we cannot call a generic method of 
            // an object.
            /*
            var e = new Employee();
            var t = typeof(Employee);
            var methodInfo = t.GetMethod("Speak");
            methodInfo.Invoke(e, null); // This fails sincet the method is generic.
             */

            // Solves: How do I invoke a generic method through reflection?
            var e = new Employee();
            var t = typeof(Employee);
            var methodInfo = t.GetMethod("Speak");
            methodInfo = methodInfo.MakeGenericMethod(t);
            methodInfo.Invoke(e, null); 
        }

        private static object CreateCollection(Type collectionType, Type itemType)
        {
            // Activator.CreateInstane works on closed types, but if the type contains
            // a generic type parameter, then MakeGenericType is to be used.
            var closedType = collectionType.MakeGenericType(itemType);
            return Activator.CreateInstance(closedType);
        }
    }

    public class Employee
    {
        public string Name { get; set; }

        public void Speak<T>()
        {
            Console.WriteLine(typeof(T).Name);
        }
    }
}
