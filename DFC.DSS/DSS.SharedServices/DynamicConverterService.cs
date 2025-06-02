using System.Dynamic;
using DSS.Interfaces;

namespace DSS.SharedServices
{
    public class DynamicConverterService : IDynamicConverterService
    {
        public ExpandoObject RenameAndExcludeProperty<T>(T model, string? oldname, string newName, string? exclName)
        {
            var updatedObject = new ExpandoObject();
            foreach (var item in typeof(T).GetProperties())
            {
                if (item.Name == exclName)
                    continue;
                var itemName = item.Name;
                if (itemName == oldname)
                    itemName = newName;
#pragma warning disable CS8604 // Possible null reference argument.
                AddProperty(updatedObject, itemName, item.GetValue(model));
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return updatedObject;
        }
        public ExpandoObject RenameProperty<T>(T model, string? name, string newName)
        {
            var updatedObject = new ExpandoObject();
            foreach (var item in typeof(T).GetProperties())
            {
                var itemName = item.Name;
                if (itemName == name)
                    itemName = newName;
#pragma warning disable CS8604 // Possible null reference argument.
                AddProperty(updatedObject, itemName, item.GetValue(model));
#pragma warning restore CS8604 // Possible null reference argument.
            }
            return updatedObject;
        }
        public IList<ExpandoObject> RenameProperty<T>(IList<T> models, string name, string newName)
        {
            var updatedObjects = new List<ExpandoObject>();
            foreach (var actionPlan in models)
            {
                updatedObjects.Add(RenameProperty(actionPlan, name, newName));
            }

            return updatedObjects;
        }
        public ExpandoObject ExcludeProperty<T>(T model, string? name)
        {
            dynamic updatedObject = new ExpandoObject();
            foreach (var item in typeof(T).GetProperties())
            {
                if (item.Name == name)
                    continue;
                AddProperty(updatedObject, item.Name, item.GetValue(model));
            }
            return updatedObject;
        }
        public ExpandoObject ExcludeProperty(Exception exception, string[] names)
        {
            dynamic updatedObject = new ExpandoObject();
            foreach (var item in typeof(Exception).GetProperties())
            {
                if (names.Contains(item.Name))
                    continue;

                AddProperty(updatedObject, item.Name, item.GetValue(exception));
            }
            return updatedObject;
        }
        public IList<ExpandoObject> ExcludeProperty<T>(IList<T> models, string name)
        {
            var updatedObjects = new List<ExpandoObject>();
            foreach (var actionPlan in models)
            {
                updatedObjects.Add(ExcludeProperty(actionPlan, name));
            }

            return updatedObjects;
        }
        public void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}
