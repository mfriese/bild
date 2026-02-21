using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.ExifFlags;

public class GetQuickTimeCreateDateSecInteractor
{
    public DateTime? Perform(MediaFile file)
    {
        GetQuickTimeCreateDateInteractor getQuickTimeCreateDate = new();
        var createDate = getQuickTimeCreateDate.Perform(file);

        GetSystemFileModifyDateInteractor getSystemFileModifyDate = new();
        var modifyDate = getSystemFileModifyDate.Perform(file);

        // if within one year time range I supposed the dates are okay
        if (createDate != null && modifyDate != null)
        {
            if (createDate.Value.AddYears(1) > modifyDate &&
                modifyDate.Value.AddYears(1) > createDate)
            {
                return createDate;
            }
        }

        return modifyDate;
    }
}
