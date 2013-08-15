function receiveData(serviceData) {
    if (serviceData != null && serviceData.length > 0) {
        //serviceData = JSON.parse(serviceData);
        if (serviceData.IsError == true && serviceData.ErrorMessage != "") {
            alert(serviceData.ErrorMessage);
        }
        else {
            if (serviceData.Result != null && serviceData.ResultType != "") {
                if (serviceData.Result.length > 0) {
                    if (serviceData.ResultType == "channel") {
                        getAllChannelsFill(serviceData.Result);
                    }
                    else if (serviceData.ResultType == "getUserChannel") {
                        getChannel(serviceData.Result);
                    }
                    else if (serviceData.ResultType == "joinCreateChannel") {
                        getChannel(serviceData.Result);
                        
                    }
                    else {
                        ajaxCallError(serviceData.Result);
                    }
                }
                else {
                    if (serviceData.ResultType == "restoreSession") {
                        restoreSessionFill(serviceData.Result);
                    }
                }
            }
        }
    }
};

function getAllChannelsFill(data) {
    jQuery.each(data, function (i, result) {
        addChannel(result.Name);
    });
}

function restoreSessionFill(data) {
    jQuery.each(data, function (i, result) {
        if (result != null) {
            if (i == "GetUserChannels") {
                jQuery.each(result, function (n, chanName) {
                    if (!$('#pnlSelectedChannels li:contains("' + chanName + '")').length) {
                        $("#pnlSelectedChannels ul").append('<li>' + chanName + '</li>');
                    }
                });
            }
            else if(result.length > 0)
                getChannel(result);
        }
    });
}

function ajaxCallError(err){
    alert(err);
}

function getChannel(channelData) {
    if (channelData.Name != null) {
        var html = '';
        html += '<ul>';
        html += '</ul>';
        $("#pnlMessages").html(html);
        if (channelData.Messages.length > 0) {
            jQuery.each(channelData.Messages, function (i, message) {
                addMessage(message);
            });
        }
        if (channelData.CreatedDateTime != null) {
            //channelData[0].CreatedDateTime = new Date(parseInt(channelData[0].CreatedDateTime.match(/\d+/)[0]));
            $('#lblCreatedDateTime').html(dateFormat(new Date(channelData.CreatedDateTime), 'dd-mmm-yyyy h:M TT'));
        }
        if (channelData.Name != null) {
            var chanName = $('#lblChannelName');
            chanName.html(channelData.Name);
            chanName.attr("tag", channelData.Id);
            if (!$('#pnlSelectedChannels li:contains("' + channelData.Name + '")').length) {
                $("#pnlSelectedChannels ul").append('<li>' + channelData.Name + '</li>');
            }
        }
        $("#pnlStart").css("display", "none");
        $("#pnlChannel").css("display", "block");
        PUBNUB.subscribe({
            channel: "NewMsgIn" + channelData.Id,
            callback: function(message) {
                if (message.ChannelId != null && message.ChannelId == $('#lblChannelName').attr("tag")) {
                    if (message.Status == "add") {
                        addMessage(message);
                    } else if (message.Status == "del") {
                        addMessage(message);
                    }
                }
            }
        });
        $.unblockUI();
    }
}

function addMessage(message) {
    var html = '';
    html += '<li id="' + message.Id + '">';
    var type = "Text";
    if (message.Type == 2) {
        type = "Link";
        html += '<div class="body"><div id="delete" tag="' + message.Id + '"></div><a href="' + message.Content + '" title="' + message.Title + '" target="' + message.Title + '">' + message.Title + '</a></div>';
    }
    else if (message.Type == 3) {
        type = "File";
        html += '<div class="body"><div id="delete" tag="' + message.Id + '"></div><a href="' + message.Content + '" title="' + message.Content + '" target="' + message.Content + '">' + message.Content + '</a></div>';
    }
    else if (message.Type == 4) {
        type = "Image";
        html += '<div class="body"><div id="delete" tag="' + message.Id + '"></div><a href="http://res.cloudinary.com/saykor/image/upload/' + message.Content + '" title="' + message.Content + '" target="' + message.Content + '"><img src="http://res.cloudinary.com/saykor/image/upload/w_150,h_100,c_fill/' + message.Content + '" alt="' + message.Content + '" class="dynamic_image"></a></div>';
    }
    else {
        html += '<div class="body"><div id="delete" tag="' + message.Id + '"></div>' + message.Content + '</div>';
    }
    //message.CreatedDateTime = new Date(parseInt(message.CreatedDateTime.match(/\d+/)[0]));
    html += '<div class="type">Message Type: ' + type + '</div><div class="date">Created Date: ' + dateFormat(new Date(message.CreatedDateTime), 'dd-mmm-yyyy h:M TT') + '</div>';
    html += '</li>';
    $("#pnlMessages ul").prepend(html);
}

function addChannel(channel) {
    $("#pnlAllChannels ul").append('<li>' + channel + '</li>');
}

String.prototype.endsWith = function (pattern) {
    var d = this.length - pattern.length;
    return d >= 0 && this.lastIndexOf(pattern) === d;
};

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

// A little helper function to ensure the url's work in asp.net development server and IIS
function getUrl(action, controller) {
    var protocol = window.location.protocol;
    var host = window.location.host;

    var url = (protocol + '//' + host + '/');

    if ((controller != null && controller != 'undefined') && (action != null && action != 'undefined'))
        url += controller + '/' + action;

    return url;
}

function deleteData(id) {
    var message = document.getElementById(id);
    message.remove();
}


String.prototype.escape = function () {
    var tagsToReplace = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;'
    };
    return this.replace(/[&<>]/g, function (tag) {
        return tagsToReplace[tag] || tag;
    });
};