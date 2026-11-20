/*
 * Child ages: every time the number of children goes up, a popup asks for that child's age.
 * Posts the ages as repeated hidden inputs:  ChildrenAges=5&ChildrenAges=9
 *
 * TajanChildAges.attach({
 *   input:     element holding the number of children (<input> or <select>)
 *   container: element where the age chips + hidden inputs are rendered
 *   setCount:  function(n) that writes n back to the UI (used when the user cancels / clamps)
 *   max:       max number of children (default 6)
 *   minAge / maxAge: allowed ages (default 2 / 12)
 *   labels:    { title, question, unit, confirm, cancel, error, chip, edit }   ({n} = child number, {age} = age)
 * })
 */
(function () {
    var modal;

    function fmt(s, n, age) { return String(s).replace('{n}', n).replace('{age}', age); }

    function ensureModal(L) {
        if (modal) return modal;
        var overlay = document.createElement('div');
        overlay.className = 'fixed inset-0 z-[200] hidden items-center justify-center bg-black/45 px-4';
        overlay.setAttribute('role', 'dialog');
        overlay.setAttribute('aria-modal', 'true');
        overlay.innerHTML =
            '<div class="w-full max-w-sm rounded-2xl bg-white p-6 shadow-2xl">' +
            '  <h4 data-role="title" class="font-display text-xl text-tajan-red"></h4>' +
            '  <p data-role="question" class="mt-1 text-sm text-tajan-red/70"></p>' +
            '  <div class="mt-4 flex items-center gap-3">' +
            '    <input data-role="age" type="text" inputmode="numeric" maxlength="2" autocomplete="off" ' +
            '           class="w-20 rounded-lg border border-tajan-red/15 px-3 py-2.5 text-center text-base font-medium text-tajan-red focus:border-tajan-gold focus:outline-none" />' +
            '    <span data-role="unit" class="text-sm text-tajan-red/70"></span>' +
            '  </div>' +
            '  <p data-role="error" class="mt-2 hidden text-xs text-red-600"></p>' +
            '  <div class="mt-6 flex justify-end gap-3">' +
            '    <button type="button" data-role="cancel" class="rounded-lg border border-tajan-red/15 px-4 py-2.5 text-sm text-tajan-red/80 hover:border-tajan-gold transition"></button>' +
            '    <button type="button" data-role="ok" class="rounded-lg bg-tajan-red px-5 py-2.5 text-sm font-semibold text-white hover:bg-tajan-red/90 transition"></button>' +
            '  </div>' +
            '</div>';
        document.body.appendChild(overlay);
        modal = { el: overlay };
        ['title', 'question', 'age', 'unit', 'error', 'cancel', 'ok'].forEach(function (r) {
            modal[r] = overlay.querySelector('[data-role="' + r + '"]');
        });
        return modal;
    }

    // Opens the popup. done(age) gets a number, or null when cancelled.
    function ask(opts, n, current, done) {
        var L = opts.labels, m = ensureModal(L), finished = false;
        m.title.textContent = fmt(L.title, n);
        m.question.textContent = L.question;
        m.unit.textContent = L.unit;
        m.cancel.textContent = L.cancel;
        m.ok.textContent = L.confirm;
        m.error.classList.add('hidden');
        m.age.value = current == null ? '' : current;
        m.el.classList.remove('hidden');
        m.el.classList.add('flex');
        setTimeout(function () { m.age.focus(); m.age.select(); }, 30);

        function close(result) {
            if (finished) return;
            finished = true;
            m.el.classList.add('hidden');
            m.el.classList.remove('flex');
            m.ok.removeEventListener('click', onOk);
            m.cancel.removeEventListener('click', onCancel);
            m.age.removeEventListener('keydown', onKey);
            m.age.removeEventListener('input', onInput);
            m.el.removeEventListener('mousedown', onBackdrop);
            done(result);
        }
        function onOk() {
            var v = parseInt(m.age.value, 10);
            if (isNaN(v) || v < opts.minAge || v > opts.maxAge) {
                m.error.textContent = L.error;
                m.error.classList.remove('hidden');
                m.age.focus();
                return;
            }
            close(v);
        }
        function onCancel() { close(null); }
        function onKey(e) {
            if (e.key === 'Enter') { e.preventDefault(); onOk(); }
            else if (e.key === 'Escape') { e.preventDefault(); onCancel(); }
        }
        function onInput() { m.age.value = m.age.value.replace(/\D/g, ''); m.error.classList.add('hidden'); }
        function onBackdrop(e) { if (e.target === m.el) onCancel(); }

        m.ok.addEventListener('click', onOk);
        m.cancel.addEventListener('click', onCancel);
        m.age.addEventListener('keydown', onKey);
        m.age.addEventListener('input', onInput);
        m.el.addEventListener('mousedown', onBackdrop);
    }

    function attach(o) {
        var opts = {
            input: o.input, container: o.container, setCount: o.setCount,
            max: o.max || 6, minAge: o.minAge || 2, maxAge: o.maxAge || 12,
            labels: Object.assign({
                title: 'Child {n}', question: "How old is this child?", unit: 'years old',
                confirm: 'Confirm', cancel: 'Cancel', error: 'Please enter an age between 2 and 12.',
                chip: 'Child {n}: {age} yrs', edit: 'Edit'
            }, o.labels || {})
        };
        var ages = [], busy = false;

        // ages already posted (e.g. the page was re-displayed after a validation error)
        (opts.container.dataset.ages || '').split(',').forEach(function (s) {
            var v = parseInt(s, 10);
            if (!isNaN(v) && v >= opts.minAge && v <= opts.maxAge && ages.length < opts.max) ages.push(v);
        });

        function count() {
            var v = parseInt(opts.input.value, 10);
            return isNaN(v) ? 0 : Math.min(opts.max, Math.max(0, v));
        }

        function render() {
            opts.container.innerHTML = '';
            ages.forEach(function (age, i) {
                var hidden = document.createElement('input');
                hidden.type = 'hidden'; hidden.name = 'ChildrenAges'; hidden.value = age;
                opts.container.appendChild(hidden);

                var chip = document.createElement('button');
                chip.type = 'button';
                chip.title = opts.labels.edit;
                chip.className = 'inline-flex items-center gap-1.5 rounded-full border border-tajan-gold/60 bg-white px-3 py-1 text-xs text-tajan-red hover:bg-tajan-gold/10 transition';
                chip.textContent = fmt(opts.labels.chip, i + 1, age);
                chip.addEventListener('click', function () {
                    if (busy) return;
                    busy = true;
                    ask(opts, i + 1, ages[i], function (v) {
                        busy = false;
                        if (v != null) { ages[i] = v; render(); }
                    });
                });
                var wrap = document.createElement('span');
                wrap.appendChild(chip);
                opts.container.appendChild(wrap);
            });
            opts.container.classList.toggle('hidden', ages.length === 0);
        }

        function askNext(target) {
            if (ages.length >= target) { busy = false; render(); return; }
            busy = true;
            ask(opts, ages.length + 1, null, function (v) {
                if (v == null) {                 // cancelled -> go back to the number of children we have ages for
                    busy = false;
                    opts.setCount(ages.length);
                    render();
                    return;
                }
                ages.push(v);
                render();
                askNext(target);
            });
        }

        function sync() {
            if (busy) return;
            var n = count();
            if (parseInt(opts.input.value, 10) !== n) opts.setCount(n);   // clamp to 0..max
            if (ages.length > n) { ages.length = n; render(); }
            else if (ages.length < n) askNext(n);
        }

        opts.input.addEventListener('change', sync);
        render();
        if (ages.length < count()) sync();
        return { sync: sync, ages: function () { return ages.slice(); } };
    }

    window.TajanChildAges = { attach: attach };
})();
