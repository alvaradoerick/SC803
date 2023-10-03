export function handleImageUpload(event, value) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => {
        value.Image = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }
  