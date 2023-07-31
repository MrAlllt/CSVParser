const $ = element => document.querySelector(element);
const getExt = filename => filename.substring(filename.lastIndexOf('.') + 1);
const removeExt = filename => filename.substring(0, filename.length - getExt(filename).length - 1);
const clearContent = element => element.innerText = "";
const toggleClass = (element, className) => element.classList.toggle(className);

const objToString = obj => {
    const properties = Object.getOwnPropertyNames(obj);
    let str = "";

    properties.forEach(property => {
        str += `${property}=${obj[property]};`;
    });

    return str;
};


const file = $('#csv-file');
const error = $('#error');
const propertiesDiv = $('#properties');
const customLinesActivator = $('#linesCustomActivator');
const firstLineProperties = $('#firstLineAsProperties')
const customLines = $('#linesCustom');
const separator = $('#separator');
const options = {
    firstLineAsProperties: true,
    separator: ',',
    numberOfLines: -1,
    customProperties: []
};

const addCustomProperties = properties => {
    if (propertiesDiv.classList.contains('not-shown'))
        toggleClass(propertiesDiv, 'not-shown');
    
    options.customProperties = [];
    let content = "";
    let key = 0;
    
    properties.split(options.separator).forEach(property => {
        content += `<input data-key="${key}" disabled onchange="changeProperties(this);" class="property" placeholder='${property}' value='${property}'>`;
        options.customProperties.push(property);
        key++;
    });
    
    propertiesDiv.innerHTML = content;
};

const download = (filename, content) => {
    const element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(content));
    element.setAttribute('download', filename);
    
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
};

const changeProperties = property => {
    const key = property.dataset.key;
    options.customProperties[key] = property.value;
    
    if (property.value.length === 0)
        options.customProperties[key] = property.placeholder;
};

customLinesActivator.addEventListener('change', () => {
    if (customLinesActivator.checked) {
        customLines.disabled = false;
        if (customLines.length < 0)
            options.numberOfLines = parseInt(customLines.value);
    } else {
        customLines.disabled = true;
        options.numberOfLines = -1;
    }
});

customLines.addEventListener('change', () => {
    if (customLines.value)
        options.numberOfLines = parseInt(customLines.value);
    else
        options.numberOfLines = -1;
});

separator.addEventListener('change', () => {
    if (separator.value)
        options.separator = separator.value;
    else
        options.separator = ',';
});

firstLineProperties.addEventListener('change', () => {
    if (!firstLineProperties.checked)
        document.querySelectorAll('.property').forEach(property => {
            property.disabled = false;
            options.firstLineAsProperties = false;
        });
    else
        document.querySelectorAll('.property').forEach(property => {
            property.disabled = true;
            options.firstLineAsProperties = true;
        });
    
});

$('#form-file').addEventListener('submit', event => {
    event.preventDefault()
    
    const formData = new FormData();
    formData.append('CsvFile', file.files[0]);
    formData.append('options', JSON.stringify(options));
    
    const fileSend = async function() {
        await fetch('/Parse', {
            method: 'POST',
            body: formData,
            credentials: "same-origin"
        })
            .then(res => res.text())
            .then(text => download(removeExt(file.files[0].name) + ".json", text))
            .catch(err => error.innerText = err);
    }
    
    if (getExt(file.value) === 'csv')
        fileSend();
    else
        error.innerText = "Incorrect file type!";
});

file.addEventListener('change', () => {
    const fileReader = new FileReader();
    const previewHeader = $('.h2--preview');
    previewHeader.innerText = "Code Preview";
    const preview = $('.preview');
    
    if (file.value !== "" && getExt(file.value) === 'csv') {
        toggleClass($('#options'), 'not-shown');
        clearContent(error);
        
        fileReader.onload = function () {
            const content = this.result;
            const firstLine = content.split('\n').shift();
            
            addCustomProperties(firstLine);
            
            let str = "";
            const contentSplit = content.split('\n');
            
            if (contentSplit.length > 10000)
                previewHeader.innerText += " - showing 10 000 lines (file too big)";
            
            for (let i = 1; i < contentSplit.length; i++) {
                const line = contentSplit[i - 1];
                str += `<div class="preview-line"><div class="line-number">${i}</div> <div class="line">${line}</div></div>`;
                
                if (i === 10000)
                    break;
            }
            
            preview.innerHTML = str;
        }
        
        fileReader.readAsText(file.files[0]);
    }
});