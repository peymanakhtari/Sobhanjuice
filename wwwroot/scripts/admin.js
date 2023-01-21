
function getImageFromFile() {
    var file = document.getElementById('exampleFormControlFile1').files[0];
    getBase64(file).then(
        data => document.getElementById('productimg').src = data
    );
    getBase64(file).then(
        data => document.getElementById('imgBase64').value = resizeImage(data, 150, 150)
    );
}
function getImageFromFileCategory() {
    var file = document.getElementById('exampleFormControlFile1').files[0];
    getBase64(file).then(
        data => document.getElementById('productimg').src = data
    );
    getBase64(file).then(
        data => document.getElementById('imgBase64').value = resizeImage(data, 80, 80)
    );

}
function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}


function resizeImage(base64Str, w, h) {
    
    var img = new Image();
    img.src = base64Str;
    document.getElementById('productimg').src = base64Str;
    var elem = document.getElementById('productimg');
    var canvas = document.createElement('canvas');
    var MAX_WIDTH = w;
    var MAX_HEIGHT = h;
    var width = elem.width;
    var height = elem.height;


    if (width > height) {
        if (width > MAX_WIDTH) {
            height *= MAX_WIDTH / width;
            width = MAX_WIDTH;
        }
    } else {
        if (height > MAX_HEIGHT) {
            width *= MAX_HEIGHT / height;
            height = MAX_HEIGHT;
        }
    }
    canvas.width = width;
    canvas.height = height;
    var ctx = canvas.getContext('2d');
    ctx.drawImage(img, 0, 0, width, height);
    return canvas.toDataURL();
}

function newOrder() {
    $.ajax({
        type: 'GET',
        url: '/AdminOrders/CheckNewOrder',
        success: function (result) {
            if (result) {
                beep();
            }
         
        }
    })
}

function ShowOrderAlarm(){
    $.ajax({
        type: 'GET',
        url: '/AdminOrders/CheckNewOrder',
        success: function (result) {
            if (result) {
                $('#newOrderAlarm').css('display','block')
            }
            else{
                $('#newOrderAlarm').css('display','none')
            }
        }
    })
}


function getOrders() {
    $.ajax({
        type: "GET",
        url: '/AdminOrders/GetOrders',
        contentType: "application/json",
        success: function (data) {
            const obj = JSON.parse(data);
            console.log(obj)
            removeElementsByClass('NotConfirmedOrder')
            removeElementsByClass('ConfirmedOrder')
            $.each(obj, function (i, item) {

                if (item.Order.Status == 1 || item.Order.Status == -1) {
                    var ListProduct = '';
                    $.each(item.Details, function (i, item) {
                        ListProduct +=
                            `
                    <div class="orderProduct"> ${item.Product.Name}  ${item.Count}</div>`

                    })
                    var submitButton;
                    if (item.Order.Status == 1) {

                        submitButton = '<button type="submit" class="btn btn-success">تایید سفارش</button>'
                    }
                    if (item.Order.Status == -1) {
                        submitButton = `<button type="button" class="btn btn-danger">درانتظار پرداخت ${item.Order.User.Wallet}</button>`
                    }
                    $('#NotConfirmedOrder').append(`<div  class="NotConfirmedOrder mt-3">
                   <span class="d-block w-100">
                   <span class="orderid">${item.Order.ID}</span>
                    <div>${item.Order.User.Name}</div>
                    <div>${item.Order.User.Mobile}</div>
                    <div>${item.Address.Text}</div>
                    <button style="float:left"  onclick="ShowLocation(this)"    data-loc="${item.Address.Coordinate}"   class=" mb-1 btn btn-primary me-1">مشاهده لوکیشن</button><br>
                   </span>
                   <span class="d-block w-100">
                  ${ListProduct}
                    <a href="/AdminOrders/EditOrder/${item.Order.ID}" class="btn btn-secondary">ویرایش</a>
                    <form style="" class="mt-1 me-1" class="d-inline-block" action="/AdminOrders/confirmOrder" method="post">
                        <input type="number" value="${item.Order.User.ID}" name="userId" hidden>
                        <input type="number" value="${item.Order.ID}" name="OrderId" hidden>

                        <span>
                     <span onclick="editeTime(${item.Order.ID},true)" class="arrowtime"><i class="fa fa-arrow-up" aria-hidden="true"></i></span>
                         <input id="time-${item.Order.ID}" name="time" placeholder="زمان" style="width:4rem ;display: inline-block;" value="50" type="number"
                    class="form-control" required>
                    <span onclick="editeTime(${item.Order.ID},false)" class="arrowtime" ><i class="fa fa-arrow-down" aria-hidden="true"></i></span>
                 </span>
                    ${submitButton}
                    </form>
                    </span>
                </div>
                `)
                }
                else if (item.Order.Status == 2) {
                    var ListProduct = '';
                    $.each(item.Details, function (i, item) {
                        ListProduct +=
                            `
                    <div class="orderProductConfirmed"> ${item.Product.Name} ${item.Count}</div>`

                    })

                    $('#ConfirmedOrder').append(` <div class="ConfirmedOrder mt-3">
                    <span  class="d-block w-100">
                    <span class="orderid">${item.Order.ID}</span>
                    <div>${item.Order.User.Name}</div>
                    <div>${item.Order.User.Mobile}</div>
                    <div>${item.Address.Text}</div>
                    <button style="float: left;" onclick="ShowLocation(this)"    data-loc="${item.Address.Coordinate}"   class=" mb-1 btn btn-primary me-1">مشاهده لوکیشن</button><br>
                    </span>
                    <span class="d-block w-100">
                    ${ListProduct}
                     <span style="float:left;margin-left:4px">
                    <span class="mt-2" style="border:1px solid black;padding: .5rem;border-radius: 4px;">${item.Order.Duration} دقیقه</span>
                    <a href="/AdminOrders/EditOrder/${item.Order.ID}" class="btn btn-secondary">ویرایش</a>
                    <a href="/AdminOrders/PrintOrder/${item.Order.ID}" class="px-3 btn btn-warning mb-1">پرینت</a>
                    <a href="/AdminOrders/DismissOrder/${item.Order.ID}" class="px-3 btn btn-danger mb-1">بستن سفارش</a>
                    </span>
                    </span>
                </div>
                `)
                }

            })
        },
        error: function (req, status, error) {
            alert('error')
        }
    })
}

function CloseOrder() {
    $.ajax({
        type: "post",
        url: '/AdminOrders/CloseOrder',
        contentType: "application/json",
        success: function (result) {

        },
        error: function (req, status, error) {
            alert('error')
        }

    })
}

function removeElementsByClass(className) {
    const elements = document.getElementsByClassName(className);
    while (elements.length > 0) {
        elements[0].parentNode.removeChild(elements[0]);
    }
}


function editeTime(id, mode) {

    var val = $('#time-' + id).val();
    if (mode) {
        $('#time-' + id).val(Number(val) + 5);
    }
    else {
        $('#time-' + id).val(Number(val) - 5);
    }
}


function beep() {
    var audio = document.getElementById('neworder');
    audio.play();
}



//-----------------Map---------------------------
function ShowLocation(identifier) {
    var Coordinate = $(identifier).data('loc');
    lat = Coordinate.split(',')[0];
    lon = Coordinate.split(',')[1];
    $('.mapdiv').css('display', 'block');
    seeThisAddressLocation(lat, lon);

}
function closeMap() {
    $('.mapdiv').css('display', 'none');
    myMap.off();
    myMap.remove();
}
var myMap;
function seeThisAddressLocation(lat, lon) {
    document.body.scrollTop = 0;

    myMap = new L.Map('map', {
        key: 'web.ifGasjief0An0kduWkx2cLe5a0p1tKJG4Oydvle4',
        maptype: 'dreamy',
        poi: true,
        traffic: false,
        center: [lat, lon],
        zoom: 18
    });
    var url = `https://api.neshan.org/v1/distance-matrix?origins=35.699756003184014,51.326407482292346&destinations=35.700522720414256,51.346318312706806`;

    url = encodeURI(url);
    var params = {
        headers: {
            'Api-Key': 'service.Ds1ViBcYt9iHssMxMUyLUHwcumV4Zqeb3nBydt84'
        },

    };
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(myMap);

    var greenIcon = new L.Icon({
        iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
        shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
        iconSize: [25, 41],
        iconAnchor: [12, 41],
        popupAnchor: [1, -34],
        shadowSize: [41, 41]
    });
    L.marker([29.626276965722678, 52.51239634002261], { icon: greenIcon }).addTo(myMap)
        .bindPopup('آبمیوه طبیعی سبحان')
        .openPopup();
    myMap.flyTo([lat, lon], 18)
    theMarker = L.marker([lat, lon]).addTo(myMap);

}
function ChangeCommentShow(id, show) {
    $.ajax({
        url: '/admin/ChangeCommentShow',
        data: { id: id, show: show },
        type: 'post',
        success: function () {
            location.reload();
        },
        error: function () {
            alert('server error')
        }

    })
}

function changeExist(id,exist){
    $.ajax({
        url:'/Admin/changeExistAjax',
        data:{id:id,exist:exist},
        type:'post',success:function(result){
            $('#'+id).html('');
             if (!result) {
                $('#'+id).html(` <button class="btn btn-success" onclick="changeExist(${id},false)">موجود</button>`)
             }
             else{
                $('#'+id).html(`<button class="btn btn-danger" onclick="changeExist(${id},true)">ناموجود</button>`)
             }
        },error(xhr, status, error){
            alert(xhr.responseText);
        }
    })
}
