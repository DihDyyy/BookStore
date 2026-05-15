// ===== BookStore Premium JavaScript =====

$(document).ready(function () {
    // === Navbar scroll effect ===
    $(window).scroll(function () {
        if ($(this).scrollTop() > 50) $('.navbar-custom').addClass('scrolled');
        else $('.navbar-custom').removeClass('scrolled');
    });

    // === Update cart badge ===
    function updateCartBadge() {
        $.get('/Cart/GetCount', function (data) {
            var badge = $('#cartBadge');
            badge.text(data.count);
            if (data.count > 0) badge.show(); else badge.hide();
        });
    }
    updateCartBadge();

    // === Ajax Search with debounce ===
    var searchTimeout;
    $('#searchInput').on('input', function () {
        var query = $(this).val().trim();
        clearTimeout(searchTimeout);
        if (query.length < 2) { $('#searchResults').removeClass('active').empty(); return; }
        searchTimeout = setTimeout(function () {
            $.get('/Book/Search', { query: query }, function (data) {
                var dropdown = $('#searchResults');
                dropdown.empty();
                if (data.length === 0) {
                    dropdown.html('<div class="p-3 text-center text-muted">Không tìm thấy kết quả</div>');
                } else {
                    data.forEach(function (book) {
                        var imgSrc = book.image ? '/images/books/' + book.image : '';
                        var imgHtml = imgSrc ? '<img src="' + imgSrc + '" alt="">' : '<div style="width:45px;height:60px;background:#f1f5f9;border-radius:6px;display:flex;align-items:center;justify-content:center;margin-right:12px"><i class="fas fa-book" style="color:#cbd5e1"></i></div>';
                        var priceHtml = book.isOnSale
                            ? '<span class="price"><span style="text-decoration:line-through;color:#94a3b8;font-size:.8rem">' + formatPrice(book.price) + '</span> ' + formatPrice(book.effectivePrice) + '</span>'
                            : '<span class="price">' + formatPrice(book.price) + '</span>';
                        dropdown.append(
                            '<a href="/Book/Details/' + book.id + '" class="search-result-item">' +
                            imgHtml + '<div class="info"><h6>' + book.title + '</h6><small>' + book.authorName + '</small></div>' + priceHtml + '</a>'
                        );
                    });
                }
                dropdown.addClass('active');
            });
        }, 300);
    });

    $(document).click(function (e) {
        if (!$(e.target).closest('.search-box').length) $('#searchResults').removeClass('active');
    });

    $('#searchInput').on('keypress', function (e) {
        if (e.which === 13) { window.location.href = '/Book?searchQuery=' + encodeURIComponent($(this).val()); }
    });

    // === Ajax Cart Operations ===
    window.addToCart = function (bookId, qty) {
        qty = qty || 1;
        $.post('/Cart/Add', { bookId: bookId, quantity: qty, ajax: true }, function (data) {
            if (data.success) { showToast(data.message, 'success'); updateCartBadge(); }
            else showToast(data.message, 'error');
        });
    };

    window.updateCartItem = function (bookId, qty) {
        $.post('/Cart/Update', { bookId: bookId, quantity: qty, ajax: true }, function (data) {
            if (data.success) {
                updateCartBadge();
                $('#itemTotal-' + bookId).text(formatPrice(data.itemTotal));
                $('#cartTotal').text(formatPrice(data.cartTotal));
            }
        });
    };

    window.removeCartItem = function (bookId) {
        if (!confirm('Bạn có chắc muốn xóa sách này?')) return;
        $.post('/Cart/Remove', { bookId: bookId, ajax: true }, function (data) {
            if (data.success) {
                $('#cartItem-' + bookId).fadeOut(300, function () { $(this).remove(); });
                updateCartBadge();
                $('#cartTotal').text(formatPrice(data.cartTotal));
                showToast('Đã xóa khỏi giỏ hàng', 'success');
                if (data.cartCount === 0) location.reload();
            }
        });
    };

    // === Quantity Controls ===
    $(document).on('click', '.qty-btn-minus', function () {
        var input = $(this).siblings('input');
        var val = parseInt(input.val()) - 1;
        if (val < 1) val = 1;
        input.val(val);
        var bookId = input.data('book-id');
        if (bookId) updateCartItem(bookId, val);
    });
    $(document).on('click', '.qty-btn-plus', function () {
        var input = $(this).siblings('input');
        var val = parseInt(input.val()) + 1;
        input.val(val);
        var bookId = input.data('book-id');
        if (bookId) updateCartItem(bookId, val);
    });

    // === Coupon ===
    window.applyCoupon = function () {
        var code = $('#couponCode').val().trim();
        if (!code) { showToast('Vui lòng nhập mã coupon', 'error'); return; }
        $.post('/Cart/ApplyCoupon', { couponCode: code }, function (data) {
            if (data.success) {
                showToast(data.message, 'success');
                $('#discountAmount').text('-' + formatPrice(data.discount));
                $('#finalTotal').text(formatPrice(data.finalTotal));
                $('#discountRow').show();
                $('#appliedCoupon').val(data.couponCode);
            } else showToast(data.message, 'error');
        });
    };

    // === Banner Slider ===
    var currentSlide = 0;
    var slides = $('.banner-slide');
    var totalSlides = slides.length;
    if (totalSlides > 1) {
        slides.hide().first().show();
        setInterval(function () {
            slides.eq(currentSlide).fadeOut(500);
            currentSlide = (currentSlide + 1) % totalSlides;
            slides.eq(currentSlide).fadeIn(500);
            $('.banner-dot').removeClass('active').eq(currentSlide).addClass('active');
        }, 5000);
        $('.banner-dot').click(function () {
            var idx = $(this).index();
            if (idx === currentSlide) return;
            slides.eq(currentSlide).fadeOut(500);
            currentSlide = idx;
            slides.eq(currentSlide).fadeIn(500);
            $('.banner-dot').removeClass('active').eq(currentSlide).addClass('active');
        });
    }

    // === Countdown Timer ===
    function updateCountdowns() {
        $('[data-countdown]').each(function () {
            var end = new Date($(this).data('countdown')).getTime();
            var now = new Date().getTime();
            var diff = end - now;
            if (diff <= 0) { $(this).html('<span class="text-danger fw-bold">Đã kết thúc</span>'); return; }
            var d = Math.floor(diff / 86400000), h = Math.floor((diff % 86400000) / 3600000),
                m = Math.floor((diff % 3600000) / 60000), s = Math.floor((diff % 60000) / 1000);
            $(this).html(
                '<div class="countdown-item"><span class="number">' + pad(d) + '</span><span class="label">Ngày</span></div>' +
                '<div class="countdown-item"><span class="number">' + pad(h) + '</span><span class="label">Giờ</span></div>' +
                '<div class="countdown-item"><span class="number">' + pad(m) + '</span><span class="label">Phút</span></div>' +
                '<div class="countdown-item"><span class="number">' + pad(s) + '</span><span class="label">Giây</span></div>'
            );
        });
    }
    if ($('[data-countdown]').length) { updateCountdowns(); setInterval(updateCountdowns, 1000); }

    // === Star Rating Input ===
    $('.star-rating-input i').click(function () {
        var val = $(this).data('value');
        $('#ratingInput').val(val);
        $(this).parent().find('i').each(function () {
            $(this).toggleClass('fas', $(this).data('value') <= val).toggleClass('far', $(this).data('value') > val);
        });
    });

    // === Payment Method Selection ===
    $('.payment-method').click(function () {
        $('.payment-method').removeClass('selected');
        $(this).addClass('selected');
        $(this).find('input[type=radio]').prop('checked', true);
    });

    // === Address Selection ===
    $(document).on('change', '.address-select', function () {
        var addr = $(this).find(':selected').data();
        if (addr && addr.fullname) {
            $('input[name="FullName"]').val(addr.fullname);
            $('input[name="Phone"]').val(addr.phone);
            $('input[name="Address"]').val(addr.address);
        }
    });

    // === Scroll Animations ===
    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) entry.target.classList.add('visible');
        });
    }, { threshold: 0.1 });
    document.querySelectorAll('.fade-up').forEach(function (el) { observer.observe(el); });

    // === Toast Notification ===
    window.showToast = function (msg, type) {
        type = type || 'success';
        var icon = type === 'success' ? 'fa-check-circle text-success' : 'fa-exclamation-circle text-danger';
        var toast = $('<div class="toast-custom ' + type + '"><i class="fas ' + icon + '"></i><span>' + msg + '</span><button class="btn-close btn-close-sm ms-auto" onclick="$(this).parent().fadeOut(200,function(){$(this).remove()})"></button></div>');
        if (!$('.toast-container').length) $('body').append('<div class="toast-container"></div>');
        $('.toast-container').append(toast);
        setTimeout(function () { toast.fadeOut(300, function () { $(this).remove(); }); }, 4000);
    };

    // Show server-side toast
    var successAlert = $('.alert-success');
    var errorAlert = $('.alert-danger');
    if (successAlert.length) { showToast(successAlert.text().trim(), 'success'); successAlert.remove(); }
    if (errorAlert.length) { showToast(errorAlert.text().trim(), 'error'); errorAlert.remove(); }

    // === Admin sidebar toggle (mobile) ===
    $('#sidebarToggle').click(function () { $('.admin-sidebar').toggleClass('open'); });

    // === Helpers ===
    function formatPrice(price) { return new Intl.NumberFormat('vi-VN').format(price) + 'đ'; }
    function pad(n) { return n < 10 ? '0' + n : n; }
});
