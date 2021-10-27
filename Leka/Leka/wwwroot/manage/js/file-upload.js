(function($) {
  'use strict';
    $(function() {
    $('.file-upload-browse').on('click', function() {
      var file = $(this).parent().parent().parent().find('.file-upload-default');
      file.trigger('click');
    });
    $('.file-upload-default').on('change', function() {
      $(this).parent().find('.form-control').val($(this).val().replace(/C:\\fakepath\\/i, ''));
    });
  });

    let fileInputs = document.querySelectorAll('.file-upload-default');
    let boxContainer = document.getElementById('box');

    for (let fileInput of fileInputs) {
        fileInput.addEventListener('change', function (e) {
            let files = e.target.files;
            for (let file of files) {
                let reader = new FileReader();
                reader.addEventListener('loadend', function (e) {
                    boxContainer.innerHTML = "";
                    let src = e.target.result;
                    let col4 = document.createElement('div');
                    let box = document.createElement('div');
                    let remove = document.createElement('i');
                    remove.classList.add('mdi', 'mdi-close');
                    col4.classList.add('col-md-4');

                    box.classList.add('image-box');

                    let img = document.createElement('img');
                    img.setAttribute('src', src);
                    img.setAttribute('width', 150);
                    col4.append(box)
                    box.append(img);
                    box.append(remove);
                    boxContainer.append(col4);

                    let removes = document.querySelectorAll('.image-box .mdi-close')
                    removes.forEach((remove,index) => {
                        remove.addEventListener('click', function () {
                            remove.parentElement.parentElement.remove();
                        });
                    })
                })

                reader.readAsDataURL(file);
            }
        })
    }

    
})(jQuery);