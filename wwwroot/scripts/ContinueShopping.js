

function seeThisAddressLocation(lat, lon) {
    document.body.scrollTop = 0;
    $('#getLocation').css('display', 'block');

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
    if (theMarker != undefined) {
        myMap.removeLayer(theMarker);
    };
    theMarker = L.marker([lat, lon]).addTo(myMap);

}

function changeAddress(selectObject) {
    var value = selectObject.value;
    console.log(value);

    $.ajax({
        type: 'post',
        url: '/User/setAddress',
        data: { Id: value },
        success: function () {
            location.reload();
        },
        error: function () {
            alert('خطای سرور')
        }
    })


}
function submitOrder() {
    if (document.getElementById('username').value.length == 0) {
        $('#username').css('border', '3px red solid');
        fire('لطفا نام خود را وارد کنید')
        return
    }
    if ($('#ActiveOrders').val() > 0) {
        fire('شما سفارش تایید نشده دارید')
        return
    }
  var minPrice=$('#minPrice').val();
  var totalPrice=$('#total_price').val();
//   console.log(minPrice);
//   console.log()
    if (+totalPrice<= +minPrice) {
       
        fire(`مبلغ سفارش زیر${ minPrice}  تومان نباشد`)
        return
    }
    if (!document.body.contains(document.getElementById('pickup')) && !document.body.contains(document.getElementById('deliveryPrice'))) {
        fire('لطفا آدرس  را انتخاب کنید')
        return
    }
    if ($('#NoPeyment').val() == 1) {
        var pickup = document.body.contains(document.getElementById('pickup'));
        SubmitWithoutPeyment(pickup);
        return
    }
    var pickup = document.body.contains(document.getElementById('pickup'));
    window.location.href = `/order/submitOrder/${pickup}`

}
function fire(val) {
    Swal.fire({
        text: val,
        confirmButtonColor: '#006b0b',
        confirmButtonText: 'متوجه شدم',
        icon: "warning",
    })
}

function SubmitWithoutPeyment(pickup) {
    Swal.fire({
        title: "پرداخت از کیف پول",
        text: "با توجه به شارژ کیف پول شما کل مبلغ از کیف پول کسر میگردد",
        icon: "info",
        buttons: true,
        dangerMode: true,
        confirmButtonText: 'ثبت سفارش',
        confirmButtonColor: '#006b0b'
    })
        .then((val) => {
            if (val.isConfirmed) {
                window.location.href = `/order/submitOrder/${pickup}`
            }

        });
}

function AddUserName(value) {
    $('#username').css('border', 'black solid 1px');
    $.ajax({
        type: 'post',
        url: '/User/AddUserName',
        data: { username: value },
        success: function () {

        },
        error: function () {
            alert('خطای سرور')
        }
    })
}





