using hotel_data;
using hotel_data.dto;

namespace hotel_business;

public class UserBuissnes 
{
    enMode mode = enMode.add;
    public Guid ID { get; set; }
    public Guid? personID { get; set; }
    public Guid? addBy { get; set; }
    public Guid? updateBy { get; set; }
    public DateTime? brithDay { get; set; }
    public bool? isVip { get; set; } = false;
    public bool? isUser { get; set; } = false;
    public PersonDto personData { get; set; }
    public string userName { get; set; }
    public string password { get; set; }
    public string? imagePath { get; set; }


    public UserDto userDataHolder
    {
        get
        {
            return new UserDto(
                userId: ID,
                brithDay: brithDay,
                isVip: isVip,
                userName: userName,
                password: password,
                personData: personData
                , addBy: addBy,
                isUser: isUser,
                updatedBy:updateBy
            );
        }
    }

    public UserBuissnes(UserDto userData, enMode mode = enMode.add)
    {
        this.ID = userData.userId;
        this.brithDay = userData.brithDay;
        this.isVip = userData.isVip;
        this.personData = userData.personData;
        this.userName = userData.userName;
        this.password = userData.password;
        this.addBy = userData.addBy;
        this.mode = mode;
        this.imagePath = userData.imagePath;
        this.isUser = userData.isUser;
        this.updateBy = userData.updatedBy;
    }

    private bool _createUser()
    {
        return UserData.createUser(userDataHolder);
    }

    private bool _updateUser()
    {
        return UserData.updateUser(userDataHolder);
    }

    public bool save()
    {
        switch (mode)
        {
            case enMode.add:
            {
                return _createUser() ? true : false;
            }
            case enMode.update: return _updateUser() ? true : false;

            default: return false;
        }
    }

    public static bool isExistByID(Guid id)
    {
        return UserData.isExist(id);
    }

    public static bool isExistByUserName(string username)
    {
        return UserData.isExist(username);
    }

    public static UserBuissnes? getUserByID(Guid id)
    {
        var user = UserData.getUser(id);
        return user == null ? null : new UserBuissnes(user, enMode.update);
    }

    public static UserBuissnes? getUserByUserNameAndPassword(string userNme, string password)
    {
        var user = UserData.getUser(userNme, password);
        return user == null ? null : new UserBuissnes(user, enMode.update);
    }

    public static UserBuissnes? getUserByUserName(string userNme)
    {
        var user = UserData.getUser(userNme);
        return user == null ? null : new UserBuissnes(user, enMode.update);
    }


    public static bool isExistByEmailAndID(string email, Guid id)
    {
        return UserData.isExistByEmailAndID(email, id);
    }

    public static bool makeVipUser(Guid id)
    {
        return UserData.vipUser(id);
    }

    

    public static List<UserDto> getAllUsers(int pageNumber)
    {
        return UserData.getUsersByPage(pageNumber: pageNumber);
    }

         
    
    public static bool isExistByPassword(string password)
    {
        return UserData.isExistByBassword(password);
    }


    public static bool deletedUser(Guid userid, Guid amdinId)
    {
        return UserData.delete(userid, amdinId);
    } 
   

}