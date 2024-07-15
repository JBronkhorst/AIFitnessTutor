function showGuide(id) {
    var guide = document.getElementById('guide-' + id);
    guide.style.display = guide.style.display === 'none' ? 'block' : 'none';
}

function showRecipe(id) {
    var recipe = document.getElementById('recipe-' + id);
    recipe.style.display = recipe.style.display === 'none' ? 'block' : 'none';
}
