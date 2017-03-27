//Ajax Functions
bringpro.services.user = bringpro.services.user || {}
//Register New User
bringpro.services.user.insertNewUser = function (data, onSuccess, onError) {
    var url = "/public/users/add";
    //standard ajax call to submit user payload for registration

    var settings = {
        cache: false
        , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , data: data
        , dataType: "json"
        , success: onSuccess
        , error: onError
        , type: "POST"
    };
    $.ajax(url, settings);


}
//Login Existing User
bringpro.services.user.login = function (data, onSuccess, onError) {
    var url = "/api/login";

    var settings = {
        cache: false
        , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , data: data
        , dataType: "json"
        , success: onSuccess
        , error: onError
        , type: "POST"
    };
    $.ajax(url, settings);

}

//Second Login ajax call, this will call UserProfileApiController
bringpro.services.user.getUserProfile = function (onAjaxSuccess, onAjaxError) {
    var url = "/api/user/profile/current";
    var settings = {
        cache: false
            , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
            , dataType: "json"
            , success: onAjaxSuccess
            , error: onAjaxError
            , type: "GET"
    };

    $.ajax(url, settings);

}
//Update UserProfile table and Aspnetuser table
bringpro.services.user.updateUserProfile = function (data, onSuccess, onError) {

    var url = "/api/user/profile/update";

    var settings = {
        cache: false
            , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
            , data: data
            , dataType: "json"
            , success: onSuccess
            , error: onError
            , type: "PUT"
    };
    $.ajax(url, settings);

}

bringpro.services.user.changePassword = function (data, onSuccess, onError) {

    var url = "/api/user/profile/changepassword";

    var settings = {
        cache: false
        , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , data: data
        , dataType: "json"
        , success: onSuccess
        , error: onError
        , type: "PUT"
    };

    $.ajax(url, settings);
}
//Steven's Test
bringpro.services.user.insertNewBringgUser = function (onSuccess, onError) {
    var url = "/public/users/test";


    var settings = {
        cache: false
        , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , dataType: "json"
        , success: onSuccess
        , error: onError
        , type: "GET"
    };

    $.ajax(url, settings);
}



