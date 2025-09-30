using Bild.Core.Features.Files;

namespace Bild.Core.Interactors.ExifFlags;

public class GetQuickTimeCreateDateSecInteractor
{
    public DateTime? Perform(MediaFile file)
    {
        // This is here because some cameras produce faulty quicktime create dates. So if
        // the quicktime date and the latest modify date are within one year of each other
        // I assume the dates are okay. If not if prefer to look at the modify date.
        GetQuickTimeCreateDateInteractor getQuickTimeCreateDate = new();
        DateTime? quickTimeDate = getQuickTimeCreateDate.Perform(file);

        GetSystemFileModifyDateInteractor getSystemFileModifyDate = new();
        DateTime? createdDate = getSystemFileModifyDate.Perform(file);

        // if within one year time range I supposed the dates a okay
        if (quickTimeDate != null && createdDate != null)
        {
            if (quickTimeDate.Value.AddYears(1) > createdDate &&
                createdDate.Value.AddYears(1) > quickTimeDate)
            {
                return quickTimeDate;
            }
        }

        // if quicktime create date is missing just use modified date - seems more robust
        if (createdDate != null)
        {
            return createdDate;
        }

        return null;
    }
}
