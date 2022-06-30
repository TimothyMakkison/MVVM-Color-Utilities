using Microsoft.Win32;

namespace MVVM_Color_Utilities.Infrastructure;

public class FileDialog : IFileDialog
{
    private readonly OpenFileDialog OpenImageDialog = new()
    { Filter = "Images| *.jpg;*.png;*.jpeg;*.bmp", Title = "Browse Images" };

    private readonly SaveFileDialog SaveImageDialog = new()
    { Filter = "JPG (*.jpg;*.jpeg)|(*.jpg;*.jpeg)", Title = "Save Image" };

    public bool OpenImageDialogBox(out string filePath)
    {
        var opened = OpenImageDialog.ShowDialog();
        filePath = OpenImageDialog.FileName;
        return opened ?? false;
    }

    public bool SaveImageDialogBox(out string filePath)
    {
        var opened = SaveImageDialog.ShowDialog();

        filePath = SaveImageDialog.FileName;
        return opened ?? false;
    }
}
