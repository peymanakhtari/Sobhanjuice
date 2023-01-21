
$(document).ready(function () {
    //setting basket based on cookie on page load
    ClearSlickCloneData();
    adjustPriceformat();
    $('.total-price').css('display', 'none')
    $('.product-description').each(function () {
        if (this.innerHTML.length == 0) {
            this.style.display = "none"
            this.nextElementSibling.style.top = '40px'
        }
    })
    $('.remove-basket button').click(function () {
        $('.count').each(function () {
            this.innerHTML = 0
        })
        $.ajax({
            type: 'post',
            url: '/Home/RemoveBasket',
        })
        AdjustShoppingBasket();
    })
    // check if user wants to login
    $('.product').click(
        function (e) {
            var senderElement = e.target;
            if ($(senderElement).hasClass('addFirst') ||
                $(senderElement).hasClass('product-addItem-runout') ||
                $(senderElement).parents('.editCount').length) {

            } else {
                this.style.opacity = .5;
                location.href = '/Product/Index/' + this.id;
            }
        })

    AdjustShoppingBasket();
    const queryString = window.location.search;
    const parameters = new URLSearchParams(queryString);
    const menuOpen = parameters.get('menuOpen');
    const loginFail = parameters.get('loginFail');
    if (menuOpen == 1) {
        $('.cd-panel').addClass('cd-panel--is-visible');
    }
    if (loginFail == 1) {
        $('.insert-mobile').css('display', 'none');
        $('#getConfirmationcode').css('display', 'block');
        $('.CodeWrong').css('display', 'block');
    }
})


function adjustPriceformat() {
    $('.price-format').each(function (i) {
        this.innerHTML = numberWithCommas(this.innerHTML);
    })
    $('.amount').each(function (i) {
        this.innerHTML = numberWithCommas(this.innerHTML);
    })
    $('body').persiaNumber();
}

function numberWithCommas(x) {
    var val = x.replace(/[۰-۹]/g, d => '۰۱۲۳۴۵۶۷۸۹'.indexOf(d));
    var withcomaEN = val.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return withcomaEN.replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d])
}

function addToBasketOrders(val) {
    var productId = val.split('-')[2];
    var orderId = val.split('-')[1];
    $.ajax({
        type: "post",
        url: '/home/editeshoppingbasket',
        data: { id: productId, edit: 'add' },
        datatype: "text",
        success: function (data) {
        },
        error: function (req, status, error) { }
    })
    $('#addDialog-' + orderId + "-" + productId).fadeIn(200)
}

function dismisAddDialog() {
    $('.AddToBasket-orders').fadeOut(200)
}
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
//-------------------------shopping basket--------------------------------

function AddItem(val) {
    var id = val.split('-')[1];
    $('#count-' + id).html(parseInt($('#count-' + id).html()) + 1);
    AdjustShoppingBasket();

    $.ajax({
        type: "post",
        url: '/home/editeshoppingbasket',
        data: { id: id, edit: 'add' },
        datatype: "text",
        success: function (data) {
        },
        error: function (req, status, error) { }
    })

}



function LowOff(val) {
    var id = val.split('-')[1];
    CurrentCount = parseInt($('#count-' + id).html());
    if (CurrentCount != 0) {
        $('#count-' + id).html(parseInt($('#count-' + id).html()) - 1)
    }
    AdjustShoppingBasket();
    AdjustShoppingBasket();
    $.ajax({
        type: "POST",
        url: '/Home/EditeShoppingBasket',
        data: { id: id, edit: 'lowOff' },
        dataType: "text",
        success: function (data) {

        },
        error: function (req, status, error) { }
    })

}

function AdjustShoppingBasket() {
    if ($('#getConfirmationcode:visible').length == 0) {
        $('.basket-table').html('');
        $('#basket-count').html(0);
        var basketCount = 0
        $('.count').each(function () {
            basketCount += parseInt(this.innerHTML);
        });
        var Facktor = 0;
        $('#basket-count').html(basketCount);
        if (basketCount == 0) {
            $('.insert-mobile').css('display', 'none');
            $('.basketIsEmpty').css('display', 'block');
            $('.total-price').css('display', 'none');
            $('.editCount').css('display', 'none');
            $('.addFirst').css('display', 'block');
            $('.remove-basket').css('display', 'none');
            $('.continue-shopping').css('display', 'none');
        } else {
            $('.insert-mobile').css('display', 'block');
            $('.basketIsEmpty').css('display', 'none');
            $('.total-price').css('display', 'block');
            $('.remove-basket').css('display', 'block');
            $('.continue-shopping').css('display', 'block');
            $('.product').each(function () {
                if (this.id != '') {
                    var productPrice = parseInt($('#' + this.id + ' > .product-price').html().replace("تومان", "").replace(',', ''));
                    var productCount = parseInt($('#' + this.id + '> .editCount span').html());
                    var totalProductPrice = productCount * productPrice;
                    Facktor += totalProductPrice;
                    var productName = $('#' + this.id + ' h3').html();
                    if (productCount != 0) {
                        $('#addFirst-' + this.id).css('display', 'none');
                        $('#editCount-' + this.id).css('display', 'block');

                        $('.basket-table').append(`<tr style="border=none;" id="product-basket-${this.id}">
                                 <td  class="edit-count-basket" >
                                <div id="addItemBasket-${this.id}" class="addItem-basket" onclick="AddItem(this.id)">
                                    <div class="fas fa-plus ">
                                    </div>
                                </div>
                                <span  id="countBasket" class=" badge bg-success badge-basket">${productCount}</span>
                                <div id="LowOff-${this.id}" class="LowOff-basket" onclick="LowOff(this.id)">
                                 <div class="fa fa-minus">
                                    </div>
                                </div>
                            </td >
                            <td class="price-basket price-format">${totalProductPrice}</td>
                            <td style=" padding-top: 1.5rem;" class="product-name">${productName}</td>
                        </tr >`);

                    } else {
                        $('#editCount-' + this.id).css('display', 'none');
                        $('#addFirst-' + this.id).css('display', 'block');
                    }

                }
            })
            $('#total-price').html(Facktor + ' تومان');

        }
        adjustPriceformat();
    }

}
function ChangeMobile() {
    location.href = '/home/index?menuOpen=1'
}
//-------------------------shopping basket--------------------------------
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
//---------------------------slike slider-----------------------------
function setMenuSlider() {
    $(function () {
        'use strict';
        $('.pageFoot').slick({
            slidesToShow: 3,
            slidesToScroll: 1,
            asNavFor: '.pageContent',
            arrows: false,
            focusOnSelect: true,
            centerMode: true,
            swipeToSlide: true,
            touchThreshold: 20,
            responsive: [{

                breakpoint: 5000,
                settings: {
                    centerPadding: '10px',
                    slidesToShow: 12,
                }
            }, {
                breakpoint: 990,
                settings: {
                    centerPadding: '10px',
                    slidesToShow: 9
                }
            },
            {
                breakpoint: 768,
                settings: {

                    centerPadding: '15px',
                    slidesToShow: 8
                }
            },


            {
                breakpoint: 600,
                settings: {

                    centerPadding: '30px',
                    slidesToShow: 7
                }
            }, {
                breakpoint: 500,
                settings: {

                    centerPadding: '50px',
                    slidesToShow: 5
                }
            }, {
                breakpoint: 450,
                settings: {

                    centerPadding: '30px',
                    slidesToShow: 5
                }
            }, {
                breakpoint: 400,
                settings: {

                    centerPadding: '10px',
                    slidesToShow: 5
                }
            }, {
                breakpoint: 320,
                settings: {

                    centerPadding: '30px',
                    slidesToShow: 3
                }
            }
            ]
        });
        $('.pageContent').slick({
            slidesToShow: 1,
            slidesToScroll: 1,
            asNavFor: '.pageFoot',
            arrows: false,
            infinite: true,
            swipeToSlide: true,
            touchThreshold: 17
        });
    });
}
setMenuSlider();

function ClearSlickCloneData() {
    $('.slick-cloned ul').empty();

}
$(window).scroll(function (e) {
    var $el = $('.fixedElement');
    var addHieght = $('.addHieght');
    var H1 =  $(".navbar").height() + $("#MainImg").height() + $('#pickup').height();
    var H2 = H1 + $(".pageContent").height() - 50;

    var isPositionFixed = ($el.css('position') == 'fixed');
    if (($(this).scrollTop() < H1) && isPositionFixed) {
        $el.css({
            'position': 'static',
            'top': '0px'
        });
        addHieght.css({
            'height': '10px'
        });

    }
    if ($(this).scrollTop() > H2 && isPositionFixed) {
        $el.css({
            'position': 'static',
            'top': '0px'
        });
        addHieght.css({
            'height': '10px',
        });
    }
    if (($(this).scrollTop() > H1 && $(this).scrollTop() < H2) && !isPositionFixed) {
        $el.css({
            'position': 'fixed',
            'top': '0px'
        });
        addHieght.css({
            'height': '130px'
        });

    }

});
//---------------------------slike slider-----------------------------
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
//-------------------------------------AJAX------------------------------------ 
var timeLeft;
var timerId;
function countdown() {
    var elem = document.getElementById('timerSms');
    if (timeLeft == 0) {
        clearTimeout(timerId);
        timeUp();
        elem.innerHTML = ''
    } else {
        elem.innerHTML = timeLeft;
        timeLeft--;
    }
}

function timeUp() {
    $("#btnTimer").removeAttr('disabled');
}
function startTimer() {
    $("#btnTimer").attr("disabled", "disabled");
    timeLeft = 40;
    timerId = setInterval(countdown, 1000);
  
}
function ResendConfirmationCode(){
    $("#btnTimer").attr("disabled", "disabled");
    timeLeft = 40;
    timerId = setInterval(countdown, 1000);
    $.ajax({
        type:"post",
        url:'/home/ResendConfirmationCode'
    })
}
function SendConfirmationCode1() {
    try {
        var mobile = $('#mobile').val();
        if (mobile.length<10||mobile.length>11) {
            fire('شماره موبایل نامعتبر')
            return
        }
        $('#btn-sendMessage').css('display', 'none');
        $('#btn-waitUntilsmsSend').css('display', 'block')
        $('#SendNumber').html(mobile)
        $.ajax({
            type: "post",
            url: '/Home/SendConfirmationCode',
            data: { Mobile: mobile,
                __RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val() }
        })
        document.getElementById('insert-mobile').style.display = 'none'
        document.getElementById('getConfirmationcode').style.display = 'block';
        startTimer()
    } catch (error) {
        $.ajax({
            type: "post",
            url: '/Home/emailError',
            data: { data: error },
            datatype: "text",
            success: function () {
            },
            error: function () {
            }
        })
    }

}



function LoginFromBasket() {
    $('#btn-UserLogin').css('display', 'none');
    $('#btn-waitUntilUserLogin').css('display', 'block');
    var code = $('#ConfirmationCode').val();
    $.ajax({
        type: 'post',
        url: '/Home/LoginFromBasket',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { Code: code,
            __RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val() },
        dataType: "text",
        success: function (result) {
            if (result == 1) {
                window.location.href = '/home/index?menuOpen=1'
            }
            else {
                $('#btn-waitUntilUserLogin').css('display', 'none');
                $('#btn-UserLogin').css('display', 'block');
                $('.CodeWrong').css('display', 'block');
            }
        },
        error: function () {
            alert('ارور سرور')
        }
    })

}
function pickup(value, id) {
    $('#' + id).prop('disabled', true);
    var DeliveryService = $('#deliveryService').val();
    if (DeliveryService == 0 && !value) {
        fire('ارسال به محل موقتا امکان پذیر نیست')
        return
    }
    $.ajax({
        type: 'post',
        url: '/order/pickup',
        data: { pickup: value },
        success: function () {
            location.reload();
        },
        error: function () {
            alert('خطای سرور')
        }
    })
}

//-------------------------------------AJAX------------------------------------ 
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
//-------------------------------------location---------------------------------
var myMap;
var theMarker = {};
function openMap() {
    document.body.scrollTop = 0;
    $('#getLocation').css('display', 'block');

    myMap = new L.Map('map', {
        key: 'web.ifGasjief0An0kduWkx2cLe5a0p1tKJG4Oydvle4',
        maptype: 'dreamy',
        poi: true,
        traffic: false,
        center: [29.626276965722678, 52.51239634002261],
        zoom: 18
    });
    var url = `https://api.neshan.org/v1/distance-matrix?origins=35.699756003184014,51.326407482292346&destinations=35.700522720414256,51.346318312706806`;

    url = encodeURI(url);
    var params = {
        headers: {
            'Api-Key': 'service.Ds1ViBcYt9iHssMxMUyLUHwcumV4Zqeb3nBydt84'
        },

    };


    var destination = $('#getCoordinate').val();
    if (destination != 0) {
        var lat = destination.split(',')[0];
        var lon = destination.split(',')[1];
        myMap.flyTo([lat, lon], 18)
        calculateDistnace(destination);
    }


    var marker = L.marker([29.626276965722678, 52.51239634002261]).addTo(myMap)

    myMap.on('move', function () {
        marker.setLatLng(myMap.getCenter());
    });

    myMap.on('dragend', function (e) {
        $('#showDelivery').html(`<div class="spinner-border " style="width:20px;height:20px"  role="status">
             <span class="sr-only">Loading...</span>
           </div>`)
        var cnt = myMap.getCenter();
        var position = marker.getLatLng();
        lat = Number(position['lat']).toFixed(5);
        lng = Number(position['lng']).toFixed(5);
        var destination = lat + ',' + lng;
        console.log(destination)
        calculateDistnace(destination);

    });




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



}
function calculateDistnace(destination) {
    var url = `https://api.neshan.org/v1/distance-matrix?origins=29.626276965722678,52.51239634002261&destinations=${destination}`;
    //urlencode the url
    url = encodeURI(url);
    var params = {
        headers: {
            'Api-Key': 'service.Ds1ViBcYt9iHssMxMUyLUHwcumV4Zqeb3nBydt84'
        },

    };
    var polylines = [];
    var resultMatrix = [];
    axios.get(url, params)
        .then(data => {
            for (var i = 0; i < Object.keys(data.data.rows).length; i++) {
                polylines[i] = [];
                resultMatrix[i] = [];
                for (var j = 0; j < Object.keys(data.data.rows[i].elements).length; j++) {
                    var value = data.data.rows[i].elements[j].distance.value;
                    setDistance(value, destination);
                }
            }
        }).catch(err => {
            console.log("error = " + err);
        });
}

function setDistance(value, destination) {
    $('#btnConfirmLocation').prop("disabled", false);
    $.ajax({
        type: 'post',
        url: '/order/calulateDistance',
        data: { distance: value },
        datatype: 'text',
        success: function (result) {
            if (result == 'notInServiceArea') {
                $('#showDelivery').html('خارج از محدوده سرویس دهی');
                $('#btnConfirmLocation').prop("disabled", true);
            } else {
                $('#showDelivery').html('هزینه ارسال : ' + result.price + ' تومان ');
                document.getElementById('tempgetCoordinate').value = destination;
                document.getElementById('tempdistance').value = result.distance;
            }
        }, error: function () {
            alert('خطای سرور')
        }
    })
}
function closeMap() {
    $('#getLocation').css('display', 'none');
    if (myMap != null) {
        myMap.remove();
        myMap = null;
    }
}
function getLocation() {
    if (navigator.geolocation) {


        navigator.geolocation.getCurrentPosition(showPosition)
        function showPosition(position) {
            var lat = position.coords.latitude;
            var lon = position.coords.longitude;
            var destination = lat + ',' + lon;

            if (theMarker != undefined) {
                myMap.removeLayer(theMarker);
            };

            calculateDistnace(destination);
            myMap.flyTo([lat, lon], 18)
        }

    } else {
        alert('لوکیشن خود را روشن کنید')
    }

}

function confirmLocation() {
    $('#getCoordinate').val($('#tempgetCoordinate').val());
    $('#distance').val($('#tempdistance').val())
    $('#getLocation').css('display', 'none');
    $('#btnGetLocation').removeClass('btn-danger');
    if (myMap != null) {
        myMap.remove();
        myMap = null;
    }
}

//-------------------------------------loation---------------------------------
//------------------------------------------------------------------------------

//-----------------------------------------------------------------------------
//---------------------------------comments-------------------------------------
function SubmitComment() {
    var rate = $('input[name="rate"]:checked').val()
    var text = $('#floatingTextarea2').val();
    var productId = $('#productId').val();

    if ($('input[name="rate"]').is(':checked')) {
        $('#btnSubmitComment').prop('disabled', 'true');
        $.ajax({
            url: '/product/submitcomment',
            type: 'post',
            data: { rate: rate, text: text, productId: productId },
            success: function () {
                $('#CommentContainer').html('');
                $('#CommentContainer').append(`<br><br><br><br> <div dir="rtl" class="col-10 col-sm-7 alert alert-success alert-dismissible">
          <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
          امتیاز و نظر شما با موفقیت ثبت شد و به زودی منتشر خواهد شد <a href="/home/index" class="btn btn-success btnReturn">بازگشت<a/>
        </div>`)

            }, error: function () {
                alert('ارور سرور');
            }
        })
    }
    else {
        fire('لطفا امتیاز دهید')
    }

}
function fire(val) {
    Swal.fire({
        text: val,
        confirmButtonColor: '#006b0b',
        confirmButtonText: 'متوجه شدم',
        icon: "warning",
    })
}

function loginFromLoginPage() {
    $('#SubmitBtnLoginPage').attr('disabled', 'disabled')
    $('#formLogin').submit();
}