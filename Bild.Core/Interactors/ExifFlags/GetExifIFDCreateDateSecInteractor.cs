using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.ExifFlags;

public class GetExifIFDCreateDateSecInteractor
{
    public DateTime? Perform(MediaFile file)
    {
        GetExifIFDCreateDateInteractor getExifIFDCreateDateInteractor = new();
        DateTime? createDate = getExifIFDCreateDateInteractor.Perform(file);

        GetSystemFileModifyDateInteractor getSystemFileModifyDate = new();
        DateTime? modifyDate = getSystemFileModifyDate.Perform(file);

        // if within one year time range I supposed the dates a okay
        if (createDate != null && modifyDate != null)
        {
            if (createDate.Value.AddYears(1) > modifyDate &&
                modifyDate.Value.AddYears(1) > createDate)
            {
                return createDate;
            }
        }

        // if quicktime create date is missing just use modified date - seems more robust
        if (modifyDate != null)
        {
            return modifyDate;
        }

        return null;
    }
}
