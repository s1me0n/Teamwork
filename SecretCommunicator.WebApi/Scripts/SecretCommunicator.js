$(document).ready(function () {

    var uploader2 = new qq.FileUploader({
        element: jQuery('#btnUploader2')[0], // The HTML element to turn into the uploader
        //action: getUrl() + 'Webservices/GenericUploadHandler.ashx?dada=nene', // Action method that will handle the upload
        action: getUrl() + 'api/dropbox',
        multiple: false, // Single or Mutliple file selection
        allowedExtensions: ['png', 'jpeg', 'jpg', 'gif', 'bmp', 'txt', 'pdf', 'doc', 'docx', 'mp3'], // File Type restrictions
        sizeLimit: 0, // Max File Size
        minSizeLimit: 0, // Min File Size
        debug: false, // When true outputs server response to browser console
        // Show a preview of the uploaded image
        onComplete: function (id, fileName, responseJSON) {
            if (fileName.endsWith('png') || fileName.endsWith('jpeg') || fileName.endsWith('jpg') || fileName.endsWith('gif') || fileName.endsWith('bmp')) {
                // Get the preview container
                var $previewContainer = jQuery('#uploader2Preview');
                // Create the preview img element
                var $preview = jQuery('<img />');
                // Add the current time to the end of the preview handler's url to prevent caching by the browser
                $preview.attr('src', getUrl() + 'Webservices/PreviewPhoto.ashx?filename=' + fileName + '&v=' + new Date().getTime());
                // Hide the preview and set it's size
                $preview.css({ display: 'none', width: '90%', height: '200px' });
                // Make sure the preview's container is empty
                $previewContainer.html('');
                // Append the preview to the container
                $previewContainer.append($preview);
                // Fade in the preview
                $preview.fadeIn('slow');
            }
        }
    });
    $('#txtWritePostTitle').validator({
        format: 'empty',
        invalidEmpty: true,
    });
    $('#txtWritePostMessage').validator({
        format: 'empty',
        invalidEmpty: true,
    });
    $('#txtLinkTitle').validator({
        format: 'empty',
        invalidEmpty: true,
    });
    $('#txtLinkURL').validator({
        format: 'url',
        invalidEmpty: true,
        error: function() {
				$('#validation_result').text('Invalid URL. Example: http://www.domain.com');
			}
    });
    $(".dynamic_image").cloudinary();
    serviceBaseUrl = "api";
    $('#btnJoinCreateChannel').click(btnJoinCreateChannel_Clicked);
    $('#btnCancel').click(btnCancel_Clicked);
    $("#btnWritePost").unbind("click").click(btnWritePost_Clicked);
    //$('#btnWritePost').live('click', function (e) {
    //    e.preventDefault;
    //    btnWritePost_Clicked(e);
    //});
    $('#btnAddLink').click(btnAddLink_Clicked);
    $("#delete").live('click',function (e) {
        e.preventDefault;
        delete_Clicked(e);
    });
    $("#pnlSelectedChannels li").live('click', function (e) {
        e.preventDefault;
        pnlSelectedChannels_Clicked(e);
    });
    $("#pnlAllChannels li").live('click', function (e) {
        e.preventDefault;
        pnlAllChannels_Clicked(e);
    });
    $("#pnlAddNewChannel").live('click', function (e) {
        e.preventDefault;
        pnlAllChannels_Clicked(e);
    });
    loadChannels();
    //restoreSession();
});

function loadChannels() {
    $.ajax({
        url: serviceBaseUrl + "/channel",
        type: "GET",
        datatype: "json",
        success: getAllChannelsFill,
        error: ajaxCallError
    });
}

//function restoreSession() {
//    $.ajax({
//        url: serviceBaseUrl + "/session",
//        type: "GET",
//        datatype: "json",
//        success: receiveData,
//        error: ajaxCallError
//    });
//}

function pnlSelectedChannels_Clicked(e) {
    var data = {
        "name": e.target.innerHTML.escape()
    };
    $.ajax({
        url: serviceBaseUrl + "/channel",
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(data),
        success: getChannel,
        error: ajaxCallError
    }); 
}

function btnJoinCreateChannel_Clicked() {
    var txtChannelName = $('#txtChannelName').val();
    var txtChannelPassword = $('#txtChannelPassword').val();
    var data = {
        "name": txtChannelName,
        "password": txtChannelPassword,
        "numberOfMessages": 10
    };
    $.ajax({
        url: serviceBaseUrl + "/channel",
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(data),
        success: getChannel,
        error: ajaxCallError
    });
}

function btnWritePost_Clicked() {
    if ($('#txtWritePostTitle').validator('validate') && $('#txtWritePostMessage').validator('validate')) {
        var txtChannelName = $('#lblChannelName').attr("tag");
        //var txtChannelPassword = $('#txtChannelPassword').val();
        var txtWritePostTitle = $('#txtWritePostTitle').val().escape();
        var txtWritePostMessage = $('#txtWritePostMessage').val().escape();
        var data = {
            "channelid": txtChannelName,
            //"password": txtChannelPassword,
            "title" :txtWritePostTitle,
            "content": txtWritePostMessage,
            "type": "text"
        };
        $.ajax({
            url: serviceBaseUrl + "/message",
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(data),
            //success: addMessage,
            error: ajaxCallError
        });
    }
}

function btnAddLink_Clicked() {
    if ($('#txtLinkURL').validator('validate') && $('#txtLinkTitle').validator('validate')) {
        var txtChannelName = $('#lblChannelName').attr("tag");
        //var txtChannelPassword = $('#txtChannelPassword').val();
        var txtLinkURL = $('#txtLinkURL').val().escape();
        var txtLinkTitle = $('#txtLinkTitle').val().escape();
        var data = {
            "channelid": txtChannelName,
            //"password": txtChannelPassword,
            "title": txtLinkTitle,
            "content": txtLinkURL,
            "type": "link"
        };
        $.ajax({
            url: serviceBaseUrl + "/message",
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(data),
            //success: receiveData,
            error: ajaxCallError
        });
    }
}

function pnlAllChannels_Clicked(e) {
    var txtChannelName = $('#txtChannelName');
    txtChannelName.attr({
        value: e.target.innerHTML
    });
    $.blockUI({ message: $('#loginForm') });
}

function btnCancel_Clicked() {
    $.unblockUI();
};

function delete_Clicked(e) {
    
    var btndelete = e.target.getAttribute("tag");
    var data = {
        "name": txtChannelName,
        "password": txtChannelPassword,
        "messageId": btndelete
    };
    
    $.ajax({
        url: serviceBaseUrl + "/message/"+btndelete,
        type: "DELETE",
        contentType: "application/json",
        success: deleteData(btndelete),
        error: ajaxCallError
    });
}