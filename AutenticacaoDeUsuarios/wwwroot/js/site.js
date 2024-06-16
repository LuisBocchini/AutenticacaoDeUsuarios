function fnMensagemSucesso(mensagem) {
    var toast = document.getElementById("toast");
    toast.textContent = mensagem;
    toast.style.color = "#155724";
    toast.style.backgroundColor = "#d4edda";
    toast.classList.add("show");
    setTimeout(function () {
        toast.classList.remove("show");
    }, 3000);
}

function fnMensagemErro(mensagem) {
    var toast = document.getElementById("toast");
    toast.textContent = mensagem;
    toast.style.color = "#721c24";
    toast.style.backgroundColor = "#f8d7da";
    toast.classList.add("show");
    setTimeout(function () {
        toast.classList.remove("show");
    }, 3000);
}

function fnDesabilitarBotao(botao, textoBotao) {
    $(botao).prop("disabled", true);
    $(botao).text(textoBotao)
    $(botao).append(`<div class="c-loader"></div>`);
    $(botao).css("opacity", "0.8");
    $(botao).css("pointer-events", "none");

}
function fnHabilitarBotao(botao, textoBotao) {
    setTimeout(function () {
        $(botao).prop("disabled", false);
        $(botao).html(textoBotao);
        $(botao).css("opacity", "1");
        $(botao).css("pointer-events", "all");
    }, 3000);
}