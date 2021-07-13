namespace MVVM_Color_Utilities.Infrastructure
{
    public interface IFileDialog
    {
        bool OpenImageDialogBox(out string filePath);

        bool SaveImageDialogBox(out string filePath);
    }
}