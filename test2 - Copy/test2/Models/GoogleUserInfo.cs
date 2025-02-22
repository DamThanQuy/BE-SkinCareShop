namespace test2.Models
{
    public class GoogleUserInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; } // Thêm thuộc tính để lưu URL hình ảnh của người dùng
        public string Id { get; set; } // Thêm thuộc tính ID của người dùng từ Google

        // Phương thức để chuyển đổi thành đối tượng User nếu cần
        public User ToUser()
        {
            return new User
            {
                Name = Email, // Sử dụng email làm tên người dùng
                // Mật khẩu có thể không cần thiết cho người dùng Google
                // Bạn có thể thêm logic để xử lý mật khẩu nếu cần
            };
        }
    }
} 