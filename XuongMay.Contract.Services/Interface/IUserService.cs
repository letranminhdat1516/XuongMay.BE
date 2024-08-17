using XuongMay.ModelViews.AuthModelViews;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IUserService
    {
        /// <summary>
        /// Lấy tất cả người dùng trong hệ thống.
        /// </summary>
        Task<IList<UserResponseModel>> GetAll();

        /// <summary>
        /// Đăng ký một người dùng mới.
        /// </summary>
        /// <param name="model">Mô hình chứa thông tin đăng ký.</param>
        /// <returns>Chuỗi kết quả nếu có lỗi, null nếu thành công.</returns>
        Task<string?> RegisterAsync(RegisterModelView model);

        /// <summary>
        /// Đăng nhập người dùng và trả về JWT token.
        /// </summary>
        /// <param name="model">Mô hình chứa thông tin đăng nhập.</param>
        /// <param name="token">JWT token trả về nếu đăng nhập thành công.</param>
        /// <returns>Chuỗi kết quả nếu có lỗi, null nếu thành công.</returns>
        Task<string?> LoginAsync(LoginModelView model);
    }
}
