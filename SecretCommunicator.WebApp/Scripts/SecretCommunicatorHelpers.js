function receiveData(serviceData) {
    if (serviceData != null) {
        serviceData = JSON.parse(serviceData);
        if (serviceData.IsError == true && serviceData.ErrorMessage != "") {
            alert(serviceData.ErrorMessage);
        }
        else {
            if (serviceData.Result != null && serviceData.ResultType != "") {
                if (serviceData.Result.length > 0) {
                    if (serviceData.ResultType == "getAllChannels") {
                        getAllChannelsFill(serviceData.Result);
                    }
                    else if (serviceData.ResultType == "getUserChannel") {
                        getChannel(serviceData.Result);
                    }
                    else if (serviceData.ResultType == "joinCreateChannel") {
                        getChannel(serviceData.Result);
                        PUBNUB.subscribe({
                            channel: "NewMsgIn" + serviceData.Result[0].Name,
                            callback: function (message) {
                                if (message.length > 1) {
                                    if (message[0].Channel == $('#lblChannelName').text()) {
                                        if (message[0].Action == "add") {
                                            addMessage(message[1]);
                                        }
                                        else if (message[0].Action == "del") {
                                            addMessage(message[1]);
                                        }
                                    }
                                }
                            }
                        });
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
        addChannel(result)
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

function ajaxCallError(err) {
    alert(err);
}

function getChannel(channelData) {
    if (channelData[0].Name != null) {
        var html = '';
        html += '<ul>';
        html += '</ul>';
        $("#pnlMessages").html(html);
        if (channelData.length > 1) {
            jQuery.each(channelData[1], function (i, message) {
                addMessage(message);
            });
        }
        if (channelData[0].CreatedDateTime != null) {
            //channelData[0].CreatedDateTime = new Date(parseInt(channelData[0].CreatedDateTime.match(/\d+/)[0]));
            $('#lblCreatedDateTime').html(dateFormat(new Date(channelData[0].CreatedDateTime), 'dd-mmm-yyyy h:M TT'));
        }
        if (channelData[0].Name != null) {
            $('#lblChannelName').html(channelData[0].Name);
            if (!$('#pnlSelectedChannels li:contains("' + channelData[0].Name + '")').length) {
                $("#pnlSelectedChannels ul").append('<li>' + channelData[0].Name + '</li>');
            }
        }
        $("#pnlStart").css("display", "none");
        $("#loginForm").css("display", "none");
        $("#pnlChannel").css("display", "block");
        $.unblockUI();
    }
}

function addMessage(message) {
    var html = '';
    html += '<li>';
    var type = "Text";
    if (message.PublicData.Type == 1) {
        type = "Link";
        if (message.PublicData.Value.split('|').length > 1) {
            var url = message.PublicData.Value.split('|')[0];
            var title = message.PublicData.Value.split('|')[1];
            html += '<div class="body"><div id="delete" tag="' + message.Id + '"></div><a href="' + url + '" title="' + title + '" target="' + title + '">' + title + '</a></div>';
        }
    }
    else if (message.PublicData.Type == 2) {
        type = "File";
        html += '<div class="body"><div id="delete" tag="' + message.Id + '"></div><a href="' + message.PublicData.Value + '" title="' + message.PublicData.Value + '" target="' + message.PublicData.Value + '">' + message.PublicData.Value + '</a></div>';
    }
    else if (message.PublicData.Type == 3) {
        type = "Image";
        html += '<div class="body"><div id="delete" tag="' + message.Id + '"></div><a href="http://res.cloudinary.com/saykor/image/upload/' + message.PublicData.Value + '" title="' + message.PublicData.Value + '" target="' + message.PublicData.Value + '"><img src="http://res.cloudinary.com/saykor/image/upload/w_150,h_100,c_fill,g_faces/' + message.PublicData.Value + '" alt="' + message.PublicData.Value + '" class="dynamic_image"></a></div>';
    }
    else {
        html += '<div class="body"><div id="delete" tag="' + message.Id + '"></div>' + message.PublicData.Value + '</div>';
    }
    //message.CreatedDateTime = new Date(parseInt(message.CreatedDateTime.match(/\d+/)[0]));
    html += '<div class="type">Message Type: ' + type + '</div><div class="date">Created Date: ' + dateFormat(new Date(message.CreatedDateTime), 'dd-mmm-yyyy h:M TT') + '</div>';
    html += '</li>';
    $("#pnlMessages ul").prepend(html);
}

function addChannel(Name) {
    $("#pnlAllChannels ul").append('<li>' + Name + '</li>');
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