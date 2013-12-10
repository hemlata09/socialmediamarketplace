using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batchprocess
{
    public class InstagramPhotoEntity : TableEntity
    {
        public InstagramPhotoEntity(Guid guid, string fourSquareId)
        {
            this.PartitionKey = fourSquareId;
            this.RowKey = guid.ToString().Trim();
        }

        public InstagramPhotoEntity() { }
        public string InstagramLocationId { get; set; }
        public string InstagramLocationName { get; set; }
        public string MediaID { get; set; }
        public string MetaDataType { get; set; }
        public string InstagramLink { get; set; }
        public string ImageTitle { get; set; }
        public string ImageUrl { get; set; }
        public string ImageHeight { get; set; }
        public string ImageWidth { get; set; }
        public string UploadedBy { get; set; }
        public string UploadedTime { get; set; }
        public string UserProfilePicture { get; set; }
        public string CreatedTime { get; set; }
        public string Comments { get; set; }
        public string CommentedBy { get; set; }
        public string CommentTime { get; set; }
    }

    public class InstagramCommentsEntity : TableEntity
    {
        public InstagramCommentsEntity(Guid guid, string fourSquareId)
        {
            this.PartitionKey = fourSquareId;
            this.RowKey = guid.ToString();
        }

        public InstagramCommentsEntity() { }
        public string InstagramLocationId { get; set; }
        public string InstagramLocationName { get; set; }
        public string MetaDataType { get; set; }
        public string InstagramLink { get; set; }
        public string ImageTitle { get; set; }
        public string ImageUrl { get; set; }
        public string ImageHeight { get; set; }
        public string ImageWidth { get; set; }
        public string UploadedBy { get; set; }
        public string UploadedTime { get; set; }
        public string UserProfilePicture { get; set; }
        public string CreatedTime { get; set; }
        public string Comments { get; set; }
        public string CommentedBy { get; set; }
        public string CommentTime { get; set; }
    }
}
