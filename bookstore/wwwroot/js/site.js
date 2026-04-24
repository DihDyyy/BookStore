// ===== BookStore - Site JavaScript =====

$(document).ready(function () {

    // ===== Update Cart Badge =====
    function updateCartBadge() {
        $.get('/Cart/GetCount', function (data) {
            var badge = $('#cartBadge');
            badge.text(data.count);
            if (data.count > 0) {
                badge.show();
            } else {
                badge.hide();
            }
        });
    }

    updateCartBadge();

    // ===== AJAX Search =====
    var searchTimer;
    var searchInput = $('#searchInput');
    var searchResults = $('#searchResults');

    searchInput.on('input', function () {
        var query = $(this).val().trim();

        clearTimeout(searchTimer);

        if (query.length < 2) {
            searchResults.removeClass('show').empty();
            return;
        }

        searchTimer = setTimeout(function () {
            $.get('/Book/Search', { query: query }, function (data) {
                searchResults.empty();

                if (data.length === 0) {
                    searchResults.html('<div class="p-3 text-center text-muted">Không tìm thấy kết quả</div>');
                } else {
                    data.forEach(function (book) {
                        var imgSrc = book.image ? '/images/books/' + book.image : '';
                        var imgHtml = imgSrc ? '<img src="' + imgSrc + '" />' : '<div style="width:40px;height:55px;background:#f0f0f0;border-radius:4px;margin-right:12px;display:flex;align-items:center;justify-content:center;"><i class="fas fa-book text-muted"></i></div>';

                        searchResults.append(
                            '<a href="/Book/Details/' + book.id + '" class="search-result-item">' +
                            imgHtml +
                            '<div>' +
                            '<strong>' + book.title + '</strong><br/>' +
                            '<small class="text-muted">' + book.authorName + '</small>' +
                            '<span class="d-block text-danger fw-bold">' + formatPrice(book.price) + '₫</span>' +
                            '</div></a>'
                        );
                    });

                    // "View all" link
                    searchResults.append(
                        '<a href="/Book?searchQuery=' + encodeURIComponent(query) + '" class="d-block text-center py-2 text-primary fw-bold" style="text-decoration:none;">Xem tất cả kết quả</a>'
                    );
                }

                searchResults.addClass('show');
            });
        }, 300);
    });

    // Click search button
    $('#searchBtn').click(function () {
        var query = searchInput.val().trim();
        if (query.length > 0) {
            window.location.href = '/Book?searchQuery=' + encodeURIComponent(query);
        }
    });

    // Enter key in search
    searchInput.keypress(function (e) {
        if (e.which === 13) {
            e.preventDefault();
            var query = $(this).val().trim();
            if (query.length > 0) {
                window.location.href = '/Book?searchQuery=' + encodeURIComponent(query);
            }
        }
    });

    // Close search results on click outside
    $(document).click(function (e) {
        if (!$(e.target).closest('#searchInput, #searchResults').length) {
            searchResults.removeClass('show');
        }
    });

    // ===== Auto-dismiss alerts =====
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // ===== Format price helper =====
    function formatPrice(price) {
        return price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
});
