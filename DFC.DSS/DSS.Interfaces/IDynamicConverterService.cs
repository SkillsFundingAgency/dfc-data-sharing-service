using System.Dynamic;

namespace DSS.Interfaces
{
    public interface IDynamicConverterService
    {
        public ExpandoObject RenameAndExcludeProperty<T>(T model, string oldname, string newName, string exclName);

        public ExpandoObject RenameProperty<T>(T model, string name, string newName);

        public IList<ExpandoObject> RenameProperty<T>(IList<T> models, string name, string newName);

        public ExpandoObject ExcludeProperty<T>(T model, string name);

        public IList<ExpandoObject> ExcludeProperty<T>(IList<T> models, string name);

        public void AddProperty(ExpandoObject expando, string propertyName, object propertyValue);

        public ExpandoObject ExcludeProperty(Exception exception, string[] names);
    }
}
