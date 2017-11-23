using System.Web;

namespace JrzAsp.Lib.MvcTools {
    public abstract class HttpRequestData<TData> {

        protected HttpRequestData() {
            DataStorageKey = GetType().FullName + ":MyData";
        }

        protected virtual string DataStorageKey { get; }

        protected TData MyData {
            get {
                if (!HttpContext.Current.Items.Contains(DataStorageKey)) {
                    var dt = InitData();
                    HttpContext.Current.Items[DataStorageKey] = dt;
                }
                return (TData) HttpContext.Current.Items[DataStorageKey];
            }
        }

        protected abstract TData InitData();
    }
}