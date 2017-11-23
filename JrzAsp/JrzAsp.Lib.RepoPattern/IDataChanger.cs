namespace JrzAsp.Lib.RepoPattern {
    public interface IDataChanger<TData, TChangerForm>
        where TData : class
        where TChangerForm : class {

        TChangerForm BuildForm(TData data);
        FurtherValidationResult ValidateFurther(TChangerForm form);
        TData PopulateData(TChangerForm form);
        void SaveChanges(TData data, TChangerForm form);
    }
}