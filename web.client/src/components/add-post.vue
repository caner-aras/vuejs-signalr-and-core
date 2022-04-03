<template>
  <b-modal id="addPostModal" ref="addPostModal" hide-footer title="New Post" @hidden="onHidden">
    <b-form @submit.prevent="onSubmit" @reset.prevent="onCancel">
      <b-form-group label="Title:" label-for="titleInput">
        <b-form-input id="titleInput"
                      type="text"
                      v-model="form.title"
                      required
                      placeholder="Post title">
        </b-form-input>
      </b-form-group>
      <b-form-group label="Content:" label-for="postInput">
        <b-form-textarea id="postInput"
                      v-model="form.body"
                      v-b-tooltip.focus.d800
                      placeholder="Say somethink"
                      :rows="6"
                      :max-rows="10">
        </b-form-textarea>
      </b-form-group>

      <button class="btn btn-primary float-right ml-2" type="submit" >Publish</button>
      <button class="btn btn-secondary float-right" type="reset">Cancel</button>
    </b-form>
  </b-modal>
</template>

<script>
export default {
  data () {
    return {
      form: {
        title: '',
        body: ''
      }
    }
  },
  methods: {
    onSubmit (evt) {
      this.$http.post('api/post', this.form).then(res => {
        this.$emit('post-added', res.data)
        this.$refs.addPostModal.hide()
      })
    },
    onCancel (evt) {
      this.$refs.addPostModal.hide()
    },
    onHidden () {
      Object.assign(this.form, {
        title: '',
        body: ''
      })
    }
  }
}
</script>
