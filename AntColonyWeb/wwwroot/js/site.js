// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Смена формата формы ввода данных
let ShowStrForm = (idCheckBox, idNumForm, idStrForm) => {
    let styleNumForm = document.getElementById(idNumForm).style
    let styleStrForm = document.getElementById(idStrForm).style

    if (document.getElementById(idCheckBox).checked) {
        styleNumForm.display = "none";
        styleStrForm.display = "inline";
    } else {
        styleNumForm.display = "inline";
        styleStrForm.display = "none";
    }
}


let idsTypesValue_Mas = [ "stringParam", "numParam", "numWithStepParam", "boolParam" ]
//Отображение форм заполнения значений для разных типов
let ShowAddValueForm = (numCheckRadio) => {
    for (let i = 0; i < idsTypesValue_Mas.length; i++) {
        let element = document.getElementById(idsTypesValue_Mas[i])
        console.log(numCheckRadio);
        if (i == numCheckRadio) {
            element.classList.add("showAddValue");
            element.classList.remove("hideAddValue");
        }
        else {
            element.classList.remove("showAddValue");
            element.classList.add("hideAddValue");
        }
    }
}