$(document).ready(function(){
    $('#submitBtn').click(function(){
        console.log("Click");
        var files = $('#attachmentInput').prop('files');
        var fileData = [];
        for(var i = 0; i < files.length; i++){
            extractFileData(files[i], function(data){
                fileData.push(data);
                console.log(data);
            });
        }
    }) ;
});

function generateBoundary(){
    var possibleChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890'()+_,-./:=?";
    var upper = 30, lower = 15;
    var length = Math.floor(Math.random() * (upper-lower + 1)) + lower;
    var boundary = "";
    for(var i = 0; i < length; i++){
        boundary += possibleChars[Math.floor(Math.random() * possibleChars.length)];
    }
    return boundary;
}

function extractFileData(file, callback){
    var reader = new FileReader();
    reader.onload = function(){
        var ret = {};
        ret.base64Text = reader.result.split(',')[1];
        ret.name = file.name;
        ret.fileName = file.name;
        ret.contentType = file.type;
        ret.encoding = "base64";
        ret.disposition = "attachment";
        callback(ret);
    };
    reader.onerror = function(error){
        console.log('Error: ', error);
    };
    reader.readAsDataURL(file);
}