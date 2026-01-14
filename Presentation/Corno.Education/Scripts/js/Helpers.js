function passWord() {
    var result = "";
    var count = 1;
    //alert(expectedPassword)
    var expectedPassword = "bvdu";
    var password = prompt('Please Enter Your Password to open time table', ' ');
    while (count < 3) {
        if (!password) {
            history.go(-1);
        }
        
    if (password.toLowerCase() === expectedPassword) {
            //alert('You Got it Right!');
            //window.open('protectpage.html');
            result = expectedPassword;
            break;
        }
        count += 1;
        password = prompt("Access Denied - Password Incorrect, Please Try Again.", "Password");
    }

    if (password.toLowerCase() !== expectedPassword & count === 3) {
        history.go(-1);
        //var url = '@Url.Action("Index", "Home")';
        //window.open(url);
        //window.location.href = url.replace('__id__', id);
        //history.go(-3);
    }
    return result;
} 

 