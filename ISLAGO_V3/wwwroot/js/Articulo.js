let categorias = [];

document.getElementById("categoriaSelect").addEventListener("change", function () {
    const id = this.value;
    const text = this.options[this.selectedIndex].text;

    if (!id || categorias.includes(id)) return;

    categorias.push(id);

    const tag = document.createElement("div");
    tag.className = "categoria-tag";
    tag.setAttribute("data-id", id);

    tag.innerHTML = `
        ${text}
        <i class="fas fa-times"></i>
    `;

    tag.querySelector("i").addEventListener("click", function () {
        categorias = categorias.filter(c => c !== id);
        tag.remove();
    });

    document.getElementById("categoriasSeleccionadas").appendChild(tag);

    this.value = "";
});